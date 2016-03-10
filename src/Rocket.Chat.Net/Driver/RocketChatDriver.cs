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
        public const string MessageTopic = "stream-messages";
        public const int MessageSubscriptionLimit = 10;

        private readonly string _url;
        private readonly bool _useSsl;
        private readonly ILogger _logger;
        private DdpClient _client;

        private readonly ConcurrentDictionary<string, StreamCollection> _collections =
            new ConcurrentDictionary<string, StreamCollection>();

        public event MessageReceived MessageReceived;
        public event DdpReconnect DdpReconnect;

        public CancellationToken TimeoutToken => CreateTimeoutToken();

        public string UserId { get; private set; }

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
            if (type == "added")
            {
                string collectionName = data.collection;
                string id = data.id;
                object field = data.fields;

                var collection = _collections.GetOrAdd(collectionName, new StreamCollection
                {
                    Name = collectionName
                });

                collection.Added(id, field);
            }

            if (type == "changed")
            {
                string collectionName = data.collection;
                string id = data.id;
                object field = data.fields;

                var collection = _collections.GetOrAdd(collectionName, new StreamCollection
                {
                    Name = collectionName
                });

                collection.Changed(id, field);
            }

            if (type == "removed")
            {
                string collectionName = data.collection;
                string id = data.id;

                var collection = _collections.GetOrAdd(collectionName, new StreamCollection
                {
                    Name = collectionName
                });

                collection.Removed(id);
            }

            var isMessage = type == "added" && data.collection == MessageTopic && data.fields.args != null;
            if (isMessage)
            {
                var messageRaw = data.fields.args[1];
                RocketMessage message = DriverHelper.ParseMessage(messageRaw);
                message.IsBotMentioned = message.Mentions.Any(x => x.Id == UserId);
                message.IsFromMyself = message.CreatedBy.Id == UserId;

                var edit = message.WasEdited ? "(EDIT)" : "";
                var mentioned = message.IsBotMentioned ? "(Mentioned)" : "";
                _logger.Info($"Message from {message.CreatedBy.Username}@{message.RoomId}{edit}{mentioned}: {message.Message}");

                OnMessageReceived(message);
            }
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
            _logger.Info($"Subscribing to Room: #{roomId}");
            await _client.SubscribeAsync(MessageTopic, TimeoutToken, roomId, MessageSubscriptionLimit.ToString());
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
            var result = ParseLogin(data);
            UserId = result.UserId;
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
            var result = ParseLogin(data);
            UserId = result.UserId;
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
                ldapOptions = new { }
            };

            var data = await _client.CallAsync("login", TimeoutToken, request);
            var result = ParseLogin(data);
            UserId = result.UserId;
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
            var result = ParseLogin(data);
            UserId = result.UserId;
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

        protected void OnMessageReceived(RocketMessage rocketmessage)
        {
            MessageReceived?.Invoke(rocketmessage);
        }

        public void Dispose()
        {
            _client.Dispose();
        }

        protected virtual void OnDdpReconnect()
        {
            DdpReconnect?.Invoke();
        }
    }
}