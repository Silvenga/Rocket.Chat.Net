namespace Rocket.Chat.Net.Driver
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Newtonsoft.Json.Linq;

    using Rocket.Chat.Net.Helpers;
    using Rocket.Chat.Net.Interfaces;
    using Rocket.Chat.Net.Models;
    using Rocket.Chat.Net.Models.Logins;
    using Rocket.Chat.Net.Models.Results;

    public class RocketChatDriver : IRocketChatDriver
    {
        private const string MessageTopic = "stream-messages";
        private const int MessageSubscriptionLimit = 10;

        private readonly ConcurrentDictionary<string, StreamCollection> _collections =
            new ConcurrentDictionary<string, StreamCollection>();

        private readonly string _url;
        private readonly bool _useSsl;
        private readonly ILogger _logger;

        private DdpClient _client;

        public event MessageReceived MessageReceived;
        public event DdpReconnect DdpReconnect;

        private CancellationToken TimeoutToken => CreateTimeoutToken();

        public string UserId { get; private set; }
        public string Username { get; private set; }

        public RocketChatDriver(string url, bool useSsl, ILogger logger)
        {
            _url = url;
            _useSsl = useSsl;
            _logger = logger;
            _logger = logger;

            Initialize();
        }

        private void Initialize()
        {
            _logger.Info("Creating client...");
            _client = new DdpClient(_url, _useSsl, _logger);
            _client.DataReceivedRaw += ClientOnDataReceivedRaw;
            _client.DdpReconnect += OnDdpReconnect;
        }

        private void ClientOnDataReceivedRaw(string type, dynamic data)
        {
            HandleCreateCollection(type, data);

            HandleRocketMessage(type, data);
        }

        private void HandleCreateCollection(string type, dynamic data)
        {
            Func<string, StreamCollection> createCollection = name => new StreamCollection
            {
                Name = name
            };

            if (type == "added")
            {
                string collectionName = data.collection;
                string id = data.id;
                object field = data.fields;

                var collection = _collections.GetOrAdd(collectionName, createCollection);
                collection.Added(id, JObject.FromObject(field));
            }

            if (type == "changed")
            {
                string collectionName = data.collection;
                string id = data.id;
                object field = data.fields;

                var collection = _collections.GetOrAdd(collectionName, createCollection);
                collection.Changed(id, JObject.FromObject(field));
            }

            if (type == "removed")
            {
                string collectionName = data.collection;
                string id = data.id;

                var collection = _collections.GetOrAdd(collectionName, createCollection);
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

        private CancellationToken CreateTimeoutToken()
        {
            _logger.Debug("Created cancellation token.");
            var source = new CancellationTokenSource();
            source.CancelAfter(TimeSpan.FromSeconds(30));

            return source.Token;
        }

        public async Task ConnectAsync()
        {
            _logger.Info($"Connecting client to {_url}...");
            await _client.ConnectAsync(TimeoutToken);
        }

        public async Task SubscribeToRoomAsync(string roomId = null)
        {
            _logger.Info($"Subscribing to Room: #{roomId ?? "ALLROOMS"}");
            await _client.SubscribeAsync(MessageTopic, TimeoutToken, roomId, MessageSubscriptionLimit.ToString());
        }

        public async Task SubscribeToFilteredUsersAsync(string username = "")
        {
            //_logger.Info($"Subscribing to filtered users searching for {username ?? "ANY"}.");
            //await _client.SubscribeAsync("filteredUsers", TimeoutToken, username);
            _logger.Info($"Subscribing to filtered users searching for {username ?? "ANY"}.");
            await _client.SubscribeAsync("userData", TimeoutToken);
        }

        public async Task PingAsync()
        {
            _logger.Info("Pinging server.");
            await _client.PingAsync(TimeoutToken);
        }

        public async Task<LoginResult> LoginAsync(ILoginOption loginOption)
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

        public async Task<LoginResult> LoginWithEmailAsync(string email, string password)
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

            var data = await _client.CallAsync("login", TimeoutToken, request);
            LoginResult result = ParseLogin(data);
            if (!result.HasError)
            {
                await SetUserInfoAsync(result.UserId);
            }
            return result;
        }

        public async Task<LoginResult> LoginWithUsernameAsync(string username, string password)
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

            var data = await _client.CallAsync("login", TimeoutToken, request);
            LoginResult result = ParseLogin(data);
            if (!result.HasError)
            {
                await SetUserInfoAsync(result.UserId);
            }
            return result;
        }

        public async Task<LoginResult> LoginWithLdapAsync(string username, string password)
        {
            _logger.Info($"Logging in with user {username} using LDAP...");
            var request = new
            {
                username,
                ldapPass = password,
                ldap = true,
                ldapOptions = new {}
            };

            var data = await _client.CallAsync("login", TimeoutToken, request);
            LoginResult result = ParseLogin(data);
            if (!result.HasError)
            {
                await SetUserInfoAsync(result.UserId);
            }

            return result;
        }

        public async Task<LoginResult> LoginResumeAsync(string sessionToken)
        {
            _logger.Info($"Resuming session {sessionToken}");
            var request = new
            {
                resume = sessionToken
            };

            var data = await _client.CallAsync("login", TimeoutToken, request);
            LoginResult result = ParseLogin(data);
            if (!result.HasError)
            {
                await SetUserInfoAsync(result.UserId);
            }
            return result;
        }

        private static LoginResult ParseLogin(dynamic data)
        {
            var result = new LoginResult();
            if (DriverHelper.HasError(data))
            {
                var error = DriverHelper.ParseError(data);
                result.ErrorData = error;
                return result;
            }

            result.UserId = data.result.id;
            result.Token = data.result.token;
            result.TokenExpires = DriverHelper.ParseDateTime(data.result.tokenExpires);

            return result;
        }

        private async Task SetUserInfoAsync(string userId)
        {
            UserId = userId;
            var collection = await WaitForCollectionAsync("users", userId, TimeoutToken);
            var user = collection.GetById<dynamic>(userId);
            string username = user.username;
            Username = username;
        }

        public async Task<string> GetRoomIdAsync(string roomIdOrName)
        {
            _logger.Info($"Looking up Room ID for: #{roomIdOrName}");
            var result = await _client.CallAsync("getRoomIdByNameOrId", TimeoutToken, roomIdOrName);
            return result?.result;
        }

        public async Task<string> DeleteMessageAsync(string messageId, string roomId)
        {
            _logger.Info($"Deleting message {messageId}");
            var request = new
            {
                rid = roomId,
                _id = messageId
            };
            return await _client.CallAsync("deleteMessage", TimeoutToken, request);
        }

        public async Task<string> CreatePrivateMessageAsync(string username)
        {
            _logger.Info($"Creating private message with {username}");
            var result = await _client.CallAsync("createDirectMessage", TimeoutToken, username);
            return result.result;
        }

        public async Task<dynamic> ChannelListAsync()
        {
            _logger.Info("Looking up public channels.");
            return await _client.CallAsync("channelsList", TimeoutToken);
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

            var rawMessage = await _client.CallAsync("loadHistory", TimeoutToken, roomId, end, limit, ls);
            var rawList = rawMessage.result.messages as JArray;
            var messages = new List<RocketMessage>();

            if (rawList == null)
            {
                return messages;
            }
            messages.AddRange(rawList.Select(DriverHelper.ParseMessage));

            return messages;
        }

        public async Task<List<RocketMessage>> SearchMessagesAsync(string query, string roomId, int limit = 100)
        {
            _logger.Info($"Searching for messages in #{roomId} using `{query}`.");

            var rawMessage = await _client.CallAsync("messageSearch", TimeoutToken, query, roomId, limit);
            var rawList = rawMessage.result.messages as JArray;
            var messages = new List<RocketMessage>();

            if (rawList == null)
            {
                return messages;
            }
            messages.AddRange(rawList.Select(DriverHelper.ParseMessage));

            return messages;
        }

        private async Task<StreamCollection> WaitForCollectionAsync(string collectionName, string id,
                                                                    CancellationToken token)
        {
            return await Task.Run(() =>
            {
                while (true)
                {
                    StreamCollection collection;
                    var success = _collections.TryGetValue(collectionName, out collection);

                    var collectonPopulated = success && collection.ContainsId(id);
                    if (collectonPopulated)
                    {
                        return collection;
                    }

                    token.ThrowIfCancellationRequested();
                    Thread.Sleep(10);
                }
            }, token);
        }

        private void OnMessageReceived(RocketMessage rocketmessage)
        {
            MessageReceived?.Invoke(rocketmessage);
        }

        public StreamCollection GetCollection(string collectionName)
        {
            StreamCollection value;
            var results = _collections.TryGetValue(collectionName, out value);

            return results ? value : null;
        }

        public void Dispose()
        {
            _client.Dispose();
        }

        protected void OnDdpReconnect()
        {
            DdpReconnect?.Invoke();
        }
    }
}