namespace Rocket.Chat.Net.Driver
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Newtonsoft.Json.Linq;

    using Rocket.Chat.Net.Bot;
    using Rocket.Chat.Net.Helpers;
    using Rocket.Chat.Net.Interfaces;
    using Rocket.Chat.Net.Models;
    using Rocket.Chat.Net.Models.LoginOptions;
    using Rocket.Chat.Net.Models.Results;

    public class RocketChatDriver : IRocketChatDriver
    {
        private const string MessageTopic = "stream-messages";
        private const int MessageSubscriptionLimit = 10;

        private readonly IStreamCollectionDatabase _collectionDatabase;
        private readonly ILogger _logger;
        private readonly IDdpClient _client;

        public event MessageReceived MessageReceived;
        public event DdpReconnect DdpReconnect;

        private CancellationToken TimeoutToken => CreateTimeoutToken();

        public string UserId { get; private set; }
        public string Username { get; private set; }

        public RocketChatDriver(string url, bool useSsl, ILogger logger = null)
        {
            _logger = logger ?? new DummyLogger();
            _collectionDatabase = new StreamCollectionDatabase();

            _logger.Info("Creating client...");
            _client = new DdpClient(url, useSsl, _logger);
            _client.DataReceivedRaw += ClientOnDataReceivedRaw;
            _client.DdpReconnect += OnDdpReconnect;
        }

        public RocketChatDriver(ILogger logger, IDdpClient client, IStreamCollectionDatabase collectionDatabaseDatabase)
        {
            _logger = logger;
            _client = client;
            _collectionDatabase = collectionDatabaseDatabase;
            _client.DataReceivedRaw += ClientOnDataReceivedRaw;
            _client.DdpReconnect += OnDdpReconnect;
        }

        private void ClientOnDataReceivedRaw(string type, dynamic data)
        {
            HandleStreamingCollections(type, data);
            HandleRocketMessage(type, data);
        }

        private void HandleStreamingCollections(string type, dynamic data)
        {
            if (type == "added")
            {
                string collectionName = data.collection;
                string id = data.id;
                object field = data.fields;

                var collection = _collectionDatabase.GetOrAddCollection(collectionName);
                collection.Added(id, JObject.FromObject(field));
            }

            if (type == "changed")
            {
                string collectionName = data.collection;
                string id = data.id;
                object field = data.fields;

                var collection = _collectionDatabase.GetOrAddCollection(collectionName);
                collection.Changed(id, JObject.FromObject(field));
            }

            if (type == "removed")
            {
                string collectionName = data.collection;
                string id = data.id;

                var collection = _collectionDatabase.GetOrAddCollection(collectionName);
                collection.Removed(id);
            }
        }

        private void HandleRocketMessage(string type, dynamic data)
        {
            var isMessage = type == "added" && data.collection == MessageTopic && data.fields.args != null;
            if (!isMessage)
            {
                return;
            }

            var messageRaw = data.fields.args[1];
            RocketMessage message = DriverHelper.ParseMessage(messageRaw);
            message.IsBotMentioned = message.Mentions.Any(x => x.Id == UserId);
            message.IsFromMyself = message.CreatedBy.Id == UserId;

            var edit = message.WasEdited ? "(EDIT)" : "";
            var mentioned = message.IsBotMentioned ? "(Mentioned)" : "";
            _logger.Info(
                $"Message from {message.CreatedBy.Username}@{message.RoomId}{edit}{mentioned}: {message.Message}");

            OnMessageReceived(message);
        }

        public async Task ConnectAsync()
        {
            _logger.Info($"Connecting client to {_client.Url}...");
            await _client.ConnectAsync(TimeoutToken);
        }

        public async Task SubscribeToRoomAsync(string roomId = null)
        {
            _logger.Info($"Subscribing to Room: #{roomId ?? "ALLROOMS"}");
            await _client.SubscribeAsync(MessageTopic, TimeoutToken, roomId, MessageSubscriptionLimit.ToString());
        }

        public async Task SubscribeToFilteredUsersAsync(string username = "")
        {
            _logger.Info($"Subscribing to filtered users searching for {username ?? "ANY"}.");
            await _client.SubscribeAsync("userData", TimeoutToken);
        }

        public async Task SubscribeToAsync(string streamName, params object[] o)
        {
            await _client.SubscribeAsync(streamName, TimeoutToken, o);
        }

        public async Task<FullUser> GetFullUserDataAsync(string username)
        {
            await _client.SubscribeAndWaitAsync("fullUserData", TimeoutToken, username, 1);

            IStreamCollection data;
            var success = _collectionDatabase.TryGetCollection("users", out data);
            if (!success)
            {
                return null;
            }

            var userPair = data
                .Items<FullUser>()
                .FirstOrDefault(x => x.Value.Username == username);
            var user = userPair.Value;
            user.Id = userPair.Key;
            return user;
        }

        public async Task PingAsync()
        {
            _logger.Info("Pinging server.");
            await _client.PingAsync(TimeoutToken);
        }

        public async Task<RocketResult<LoginResult>> LoginAsync(ILoginOption loginOption)
        {
            var ldapLogin = loginOption as LdapLoginOption;
            if (ldapLogin != null)
            {
                return await LoginWithLdapAsync(ldapLogin.Username, ldapLogin.Password);
            }
            var emailLogin = loginOption as EmailLoginOption;
            if (emailLogin != null)
            {
                return await LoginWithEmailAsync(emailLogin.Email, emailLogin.Password);
            }
            var usernameLogin = loginOption as UsernameLoginOption;
            if (usernameLogin != null)
            {
                return await LoginWithUsernameAsync(usernameLogin.Username, usernameLogin.Password);
            }
            var resumeLogin = loginOption as ResumeLoginOption;
            if (resumeLogin != null)
            {
                return await LoginResumeAsync(resumeLogin.Token);
            }

            throw new NotSupportedException($"The given login option `{typeof(ILoginOption)}` is not supported.");
        }

        public async Task<RocketResult<LoginResult>> LoginWithEmailAsync(string email, string password)
        {
            _logger.Info($"Logging in with user {email} using a email...");
            var passwordHash = DriverHelper.Sha256Hash(password);
            var request = new
            {
                user = new
                {
                    email
                },
                password = new
                {
                    digest = passwordHash,
                    algorithm = DriverHelper.Sha256
                }
            };

            return await InternalLoginAsync(request);
        }

        public async Task<RocketResult<LoginResult>> LoginWithUsernameAsync(string username, string password)
        {
            _logger.Info($"Logging in with user {username} using a username...");
            var passwordHash = DriverHelper.Sha256Hash(password);
            var request = new
            {
                user = new
                {
                    username
                },
                password = new
                {
                    digest = passwordHash,
                    algorithm = DriverHelper.Sha256
                }
            };

            return await InternalLoginAsync(request);
        }

        public async Task<RocketResult<LoginResult>> LoginWithLdapAsync(string username, string password)
        {
            _logger.Info($"Logging in with user {username} using LDAP...");
            var request = new
            {
                username,
                ldapPass = password,
                ldap = true,
                ldapOptions = new {}
            };

            return await InternalLoginAsync(request);
        }

        public async Task<RocketResult<LoginResult>> LoginResumeAsync(string sessionToken)
        {
            _logger.Info($"Resuming session {sessionToken}");
            var request = new
            {
                resume = sessionToken
            };

            return await InternalLoginAsync(request);
        }

        private async Task<RocketResult<LoginResult>> InternalLoginAsync(object request)
        {
            var data = await _client.CallAsync("login", TimeoutToken, request);
            var result = data.ToObject<RocketResult<LoginResult>>();
            if (!result.HasError)
            {
                await SetUserInfoAsync(result.Result.UserId);
            }
            return result;
        }

        private async Task SetUserInfoAsync(string userId)
        {
            UserId = userId;
            var collection = await _collectionDatabase.WaitForCollectionAsync("users", userId, TimeoutToken);
            var user = collection.GetDynamicById(userId);
            string username = user.username;
            Username = username;
        }

        public async Task<string> GetRoomIdAsync(string roomIdOrName)
        {
            _logger.Info($"Looking up Room ID for: #{roomIdOrName}");
            var result = await _client.CallAsync("getRoomIdByNameOrId", TimeoutToken, roomIdOrName);
            return result?["result"].ToObject<string>();
        }

        public async Task DeleteMessageAsync(string messageId, string roomId)
        {
            _logger.Info($"Deleting message {messageId}");
            var request = new
            {
                rid = roomId,
                _id = messageId
            };
            await _client.CallAsync("deleteMessage", TimeoutToken, request);
        }

        public async Task<string> CreatePrivateMessageAsync(string username)
        {
            _logger.Info($"Creating private message with {username}");
            dynamic result = await _client.CallAsync("createDirectMessage", TimeoutToken, username);
            return result.result;
        }

        public async Task<RocketResult<ChannelListResult>> ChannelListAsync()
        {
            _logger.Info("Looking up public channels.");
            var result = await _client.CallAsync("channelsList", TimeoutToken);
            return result.ToObject<RocketResult<ChannelListResult>>();
        }

        public async Task<dynamic> JoinRoomAsync(string roomId)
        {
            _logger.Info($"Joining Room: #{roomId}");
            return await _client.CallAsync("joinRoom", TimeoutToken, roomId);
        }

        public async Task<dynamic> SendMessageAsync(string text, string roomId)
        {
            _logger.Info($"Sending message to #{roomId}: {text}");
            var request = new
            {
                msg = text,
                rid = roomId,
                bot = true
            };
            return await _client.CallAsync("sendMessage", TimeoutToken, request);
        }

        public async Task<dynamic> UpdateMessageAsync(string messageId, string roomId, string newMessage)
        {
            _logger.Info($"Updating message {messageId}");
            var request = new
            {
                msg = newMessage,
                rid = roomId,
                bot = true,
                _id = messageId
            };
            return await _client.CallAsync("updateMessage", TimeoutToken, request);
        }

        public async Task<List<RocketMessage>> LoadMessagesAsync(string roomId, DateTime? end = null, int? limit = 20,
                                                                 string ls = null)
        {
            _logger.Info($"Loading messages from #{roomId}");

            dynamic rawMessage = await _client.CallAsync("loadHistory", TimeoutToken, roomId, end, limit, ls);
            var rawList = rawMessage.result.messages as JArray;
            var messages = new List<RocketMessage>();

            if (rawList == null)
            {
                return messages;
            }
            messages.AddRange(rawList.Cast<JObject>().Select(DriverHelper.ParseMessage));

            return messages;
        }

        public async Task<List<RocketMessage>> SearchMessagesAsync(string query, string roomId, int limit = 100)
        {
            _logger.Info($"Searching for messages in #{roomId} using `{query}`.");

            dynamic rawMessage = await _client.CallAsync("messageSearch", TimeoutToken, query, roomId, limit);
            var rawList = rawMessage.result.messages as JArray;
            var messages = new List<RocketMessage>();

            if (rawList == null)
            {
                return messages;
            }
            messages.AddRange(rawList.Cast<JObject>().Select(DriverHelper.ParseMessage));

            return messages;
        }

        public async Task<RocketResult<StatisticsResult>> GetStatisticsAsync(bool refresh = false)
        {
            _logger.Info("Requesting statistics.");
            var results = await _client.CallAsync("getStatistics", TimeoutToken);

            return results.ToObject<RocketResult<StatisticsResult>>();
        }

        public async Task<RocketResult<CreateRoomResult>> CreateRoomAsync(string roomName, IList<string> members = null)
        {
            _logger.Info($"Creating room {roomName}.");
            var results =
                await _client.CallAsync("createChannel", TimeoutToken, roomName, members ?? new List<string>());

            return results.ToObject<RocketResult<CreateRoomResult>>();
        }

        public async Task<RocketResult<CreateRoomResult>> HideRoomAsync(string roomId)
        {
            _logger.Info($"Hiding room {roomId}.");
            var results =
                await _client.CallAsync("hideRoom", TimeoutToken, roomId);

            return results.ToObject<RocketResult<CreateRoomResult>>();
        }

        public async Task<RocketResult<int>> EraseRoomAsync(string roomId)
        {
            _logger.Info($"Deleting room {roomId}.");
            var results =
                await _client.CallAsync("eraseRoom", TimeoutToken, roomId);

            return results.ToObject<RocketResult<int>>();
        }

        private void OnMessageReceived(RocketMessage rocketmessage)
        {
            MessageReceived?.Invoke(rocketmessage);
        }

        public IStreamCollection GetCollection(string collectionName)
        {
            IStreamCollection value;
            var results = _collectionDatabase.TryGetCollection(collectionName, out value);

            return results ? value : null;
        }

        public void Dispose()
        {
            _client.Dispose();
        }

        private void OnDdpReconnect()
        {
            DdpReconnect?.Invoke();
        }

        private CancellationToken CreateTimeoutToken()
        {
            _logger.Debug("Created cancellation token.");
            const int timeoutSeconds = 30;
            var source = new CancellationTokenSource();
            source.CancelAfter(TimeSpan.FromSeconds(timeoutSeconds));

            return source.Token;
        }
    }
}