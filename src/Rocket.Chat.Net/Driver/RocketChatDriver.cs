namespace Rocket.Chat.Net.Driver
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Design;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using JetBrains.Annotations;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    using Rocket.Chat.Net.Collections;
    using Rocket.Chat.Net.Helpers;
    using Rocket.Chat.Net.Interfaces;
    using Rocket.Chat.Net.Loggers;
    using Rocket.Chat.Net.Models;
    using Rocket.Chat.Net.Models.Collections;
    using Rocket.Chat.Net.Models.LoginOptions;
    using Rocket.Chat.Net.Models.MethodResults;
    using Rocket.Chat.Net.Models.SubscriptionResults;

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

        public bool IsBot { get; set; }

        public RoomCollection Rooms => new RoomCollection(GetRoomsCollection(), GetRoomInfoCollection());

        public JsonSerializerSettings JsonSerializerSettings { get; private set; }
        public JsonSerializer JsonSerializer => JsonSerializer.Create(JsonSerializerSettings);

        public RocketChatDriver(string url, bool useSsl, ILogger logger = null, bool isBot = true, JsonSerializerSettings jsonSerializerSettings = null)
        {
            IsBot = isBot;
            _logger = logger ?? new DummyLogger();
            _collectionDatabase = new StreamCollectionDatabase();

            _logger.Info("Creating client...");
            _client = new DdpClient(url, useSsl, _logger);
            _client.DataReceivedRaw += ClientOnDataReceivedRaw;
            _client.DdpReconnect += OnDdpReconnect;
            SetJsonOptions(jsonSerializerSettings);
        }

        public RocketChatDriver(ILogger logger, IDdpClient client, IStreamCollectionDatabase collectionDatabaseDatabase, bool isBot = true,
                                JsonSerializerSettings jsonSerializerSettings = null)
        {
            IsBot = isBot;
            _logger = logger;
            _client = client;
            _collectionDatabase = collectionDatabaseDatabase;
            _client.DataReceivedRaw += ClientOnDataReceivedRaw;
            _client.DdpReconnect += OnDdpReconnect;
            SetJsonOptions(jsonSerializerSettings);
        }

        private void SetJsonOptions(JsonSerializerSettings jsonSerializerSettings = null)
        {
            JsonSerializerSettings = jsonSerializerSettings ?? new JsonSerializerSettings
            {
                Error = (sender, args) =>
                {
                    _logger.Error("Handled error on (de)serialization. Please report this error to the developer: " + args.ErrorContext.Error.ToString());
                    args.ErrorContext.Handled = true;
                }
            };
        }

        private void ClientOnDataReceivedRaw(string type, JObject data)
        {
            HandleStreamingCollections(type, data);
            HandleRocketMessage(type, data);
        }

        private void HandleStreamingCollections(string type, JObject data)
        {
            var collectionResult = data.ToObject<CollectionResult>(JsonSerializer);
            if (collectionResult.Name == null)
            {
                return;
            }

            var collection = _collectionDatabase.GetOrAddCollection(collectionResult.Name);

            switch (type)
            {
                case "added":
                    collection.Added(collectionResult.Id, collectionResult.Fields);
                    break;
                case "changed":
                    collection.Changed(collectionResult.Id, collectionResult.Fields);
                    break;
                case "removed":
                    collection.Removed(collectionResult.Id);
                    break;
                default:
                    throw new InvalidOperationException($"Encountered a unknown subscription update type {type}.");
            }
        }

        private void HandleRocketMessage(string type, JObject data)
        {
            var o = data.ToObject<SubscriptionResult<JObject>>(JsonSerializer);
            var isMessage = type == "added" && o.Collection == MessageTopic && o.Fields["args"] != null;
            if (!isMessage)
            {
                return;
            }

            var messageRaw = o.Fields["args"][1];
            var message = messageRaw.ToObject<RocketMessage>(JsonSerializer);
            message.IsBotMentioned = message.Mentions.Any(x => x.Id == UserId);
            message.IsFromMyself = message.CreatedBy.Id == UserId;

            var rooms = Rooms;

            message.Room = rooms.FirstOrDefault(x => x.Id == message.RoomId);

            var edit = message.WasEdited ? "(EDIT)" : "";
            var mentioned = message.IsBotMentioned ? "(Mentioned)" : "";
            _logger.Info(
                $"Message from {message.CreatedBy.Username}@{message.RoomId}{edit}{mentioned}: {message.Message}");

            OnMessageReceived(message);
        }

        public async Task ConnectAsync()
        {
            _logger.Info($"Connecting client to {_client.Url}...");
            await _client.ConnectAsync(TimeoutToken).ConfigureAwait(false);
        }

        public async Task<MethodResult<CreateRoomResult>> CreateGroupAsync(string groupName, IList<string> members = null)
        {
            var results =
                await _client.CallAsync("createPrivateGroup", TimeoutToken, groupName, members ?? new List<string>()).ConfigureAwait(false);

            return results.ToObject<MethodResult<CreateRoomResult>>(JsonSerializer);
        }

        public async Task SubscribeToRoomListAsync()
        {
            await _client.SubscribeAndWaitAsync("subscription", TimeoutToken).ConfigureAwait(false);
            var roomCollection = GetRoomsCollection();
            if (roomCollection == null)
            {
                _logger.Error("RoomCollection should not be null.");
                return;
            }
            roomCollection.Modified += async (sender, args) =>
            {
                if (args.ModificationType == ModificationType.Added)
                {
                    var room = args.Result;
                    await SubscribeToRoomInformationAsync(room.Name, room.Type).ConfigureAwait(false);
                }
            };
            foreach (var room in roomCollection.Items().ToList().Select(x => x.Value))
            {
                await SubscribeToRoomInformationAsync(room.Name, room.Type).ConfigureAwait(false);
            }
        }

        public async Task SubscribeToRoomAsync(string roomId = null)
        {
            List<string> ids = new List<string>();

            // Subscribe to all rooms when roomId is null
            if (roomId == null)
            {
                var methodResult = await GetAvailableRoomInfoCollection().ConfigureAwait(false);
                ids.AddRange(methodResult.Result.Select(roomInfo => roomInfo.Id));
            } else
            // Subsribe to given roomId 
            {
                ids.Add(roomId);
            }

            List<Task> tasks = new List<Task>();
            foreach (string id in ids)
            {
                _logger.Info($"Subscribing to Room: #{id}");
                Task task = new Task(() => _client.SubscribeAsync(MessageTopic, TimeoutToken, id, MessageSubscriptionLimit.ToString()));
                tasks.Add(task);
                task.Start();
            }

            await Task.WhenAll(tasks).ConfigureAwait(false);
            
        }

        public async Task SubscribeToRoomInformationAsync(string roomName, RoomType type)
        {
            await _client.SubscribeAndWaitAsync("room", TimeoutToken, $"{(char) type}{roomName}").ConfigureAwait(false);
        }

        public async Task SubscribeToFilteredUsersAsync(string username = "")
        {
            _logger.Info($"Subscribing to filtered users searching for {username ?? "ANY"}.");
            await _client.SubscribeAsync("userData", TimeoutToken).ConfigureAwait(false);
        }

        public async Task SubscribeToAsync(string streamName, params object[] o)
        {
            await _client.SubscribeAsync(streamName, TimeoutToken, o).ConfigureAwait(false);
        }

        public async Task<FullUser> GetFullUserDataAsync(string username)
        {
            await _client.SubscribeAndWaitAsync("fullUserData", TimeoutToken, username, 1).ConfigureAwait(false);

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
            if (user != null)
            {
                user.Id = userPair.Key;
            }
            return user;
        }

        public async Task PingAsync()
        {
            _logger.Info("Pinging server.");
            await _client.PingAsync(TimeoutToken).ConfigureAwait(false);
        }

        public async Task<MethodResult<LoginResult>> LoginAsync(ILoginOption loginOption)
        {
            var ldapLogin = loginOption as LdapLoginOption;
            if (ldapLogin != null)
            {
                return await LoginWithLdapAsync(ldapLogin).ConfigureAwait(false);
            }
            var emailLogin = loginOption as EmailLoginOption;
            if (emailLogin != null)
            {
                return await LoginWithEmailAsync(emailLogin).ConfigureAwait(false);
            }
            var usernameLogin = loginOption as UsernameLoginOption;
            if (usernameLogin != null)
            {
                return await LoginWithUsernameAsync(usernameLogin).ConfigureAwait(false);
            }
            var resumeLogin = loginOption as ResumeLoginOption;
            if (resumeLogin != null)    
            {
                return await LoginResumeAsync(resumeLogin).ConfigureAwait(false);
            }

            throw new NotSupportedException($"The given login option `{loginOption.GetType()}` is not supported.");
        }

        public async Task<MethodResult<LoginResult>> LoginWithEmailAsync(EmailLoginOption login)
        {
            string email = login.Email;
            string password = login.Password;

            _logger.Info($"Logging in with user {email} using an email...");
            var passwordHash = EncodingHelper.Sha256Hash(password);
            var request = new
            {
                user = new
                {
                    email
                },
                password = new
                {
                    digest = passwordHash,
                    algorithm = EncodingHelper.Sha256
                }
            };

            return await InternalLoginAsync(request).ConfigureAwait(false);
        }

        public async Task<MethodResult<LoginResult>> LoginWithUsernameAsync(UsernameLoginOption login)
        {
            string username = login.Username;
            string password = login.Password;

            _logger.Info($"Logging in with user {username} using a username...");
            var passwordHash = EncodingHelper.Sha256Hash(password);
            var request = new
            {
                user = new
                {
                    username
                },
                password = new
                {
                    digest = passwordHash,
                    algorithm = EncodingHelper.Sha256
                }
            };

            return await InternalLoginAsync(request).ConfigureAwait(false);
        }

        public async Task<MethodResult<LoginResult>> LoginWithLdapAsync(LdapLoginOption login)
        {
            string username = login.Username;
            string password = login.Password;

            _logger.Info($"Logging in with user {username} using LDAP...");
            var request = new
            {
                username,
                ldapPass = password,
                ldap = true,
                ldapOptions = new {}
            };

            return await InternalLoginAsync(request).ConfigureAwait(false);
        }

        public async Task<MethodResult<LoginResult>> LoginResumeAsync(ResumeLoginOption login)
        {
            string sessionToken = login.Token;

            _logger.Info($"Resuming session {sessionToken}");
            var request = new
            {
                resume = sessionToken
            };

            return await InternalLoginAsync(request).ConfigureAwait(false);
        }

        public async Task<MethodResult<LoginResult>> GetNewTokenAsync()
        {
            var result = await _client.CallAsync("getNewToken", TimeoutToken).ConfigureAwait(false);
            var loginResult = result.ToObject<MethodResult<LoginResult>>(JsonSerializer);
            if (!loginResult.HasError)
            {
                await SetDriverUserInfoAsync(loginResult.Result.UserId).ConfigureAwait(false);
            }

            return loginResult;
        }

        public async Task<MethodResult> RemoveOtherTokensAsync()
        {
            var result = await _client.CallAsync("removeOtherTokens", TimeoutToken).ConfigureAwait(false);
            return result.ToObject<MethodResult>(JsonSerializer);
        }

        private async Task<MethodResult<LoginResult>> InternalLoginAsync(object request)
        {
            var data = await _client.CallAsync("login", TimeoutToken, request).ConfigureAwait(false);
            var result = data.ToObject<MethodResult<LoginResult>>(JsonSerializer);
            if (!result.HasError)
            {
                await SetDriverUserInfoAsync(result.Result.UserId).ConfigureAwait(false);
            }
            return result;
        }

        /// <summary>
        /// This method will retry the request with a totp code additionally sent to the server.
        /// https://developer.rocket.chat/reference/api/realtime-api/2fa
        /// </summary>
        /// <param name="originalRequest"></param>
        /// <param name="totpCode"></param>
        /// <returns></returns>
        private async Task<MethodResult<LoginResult>> InternalTotpAsync(MethodResult<LoginResult> errorResponse, object originalRequest, string totpCode)
        {
            var data = await _client.CallAsync("callWithTwoFactorRequired ", TimeoutToken, originalRequest).ConfigureAwait(false);
            var result = data.ToObject<MethodResult<LoginResult>>(JsonSerializer);
            return result;
        }

        private async Task SetDriverUserInfoAsync(string userId)
        {
            UserId = userId;
            var collection = await _collectionDatabase.WaitForObjectInCollectionAsync("users", userId, TimeoutToken).ConfigureAwait(false);
            var user = collection.GetById<FullUser>(userId);
            Username = user?.Username;
        }

        public async Task<JObject> RegisterUserAsync(string name, string emailOrUsername, string password)
        {
            var obj = new Dictionary<string, string>
            {
                {"name", name},
                {"emailOrUsername", emailOrUsername},
                {"pass", password},
                {"confirm-pass", password}
            };

            var result = await _client.CallAsync("registerUser", TimeoutToken, obj).ConfigureAwait(false);
            return result?["result"].ToObject<JObject>(JsonSerializer);
        }

        public async Task<MethodResult> SetReactionAsync(string reaction, string messageId)
        {
            var result = await _client.CallAsync("setReaction", TimeoutToken, reaction, messageId).ConfigureAwait(false);
            return result.ToObject<MethodResult>(JsonSerializer);
        }

        public async Task<MethodResult<string>> GetRoomIdAsync(string roomIdOrName)
        {
            _logger.Info($"Looking up Room ID for: #{roomIdOrName}");
            var result = await _client.CallAsync("getRoomIdByNameOrId", TimeoutToken, roomIdOrName).ConfigureAwait(false);

            return result.ToObject<MethodResult<string>>(JsonSerializer);
        }

        public async Task<MethodResult> DeleteMessageAsync(string messageId, string roomId)
        {
            _logger.Info($"Deleting message {messageId}");
            var request = new
            {
                rid = roomId,
                _id = messageId
            };
            var result = await _client.CallAsync("deleteMessage", TimeoutToken, request).ConfigureAwait(false);
            return result.ToObject<MethodResult>(JsonSerializer);
        }

        public async Task<MethodResult<CreateRoomResult>> CreatePrivateMessageAsync(string username)
        {
            _logger.Info($"Creating private message with {username}");
            var result = await _client.CallAsync("createDirectMessage", TimeoutToken, username).ConfigureAwait(false);
            return result.ToObject<MethodResult<CreateRoomResult>>(JsonSerializer);
        }

        public async Task<MethodResult<ChannelListResult>> ChannelListAsync()
        {
            _logger.Info("Looking up public channels.");
            var result = await _client.CallAsync("channelsList", TimeoutToken).ConfigureAwait(false);
            return result.ToObject<MethodResult<ChannelListResult>>(JsonSerializer);
        }

        public async Task<MethodResult> JoinRoomAsync(string roomId)
        {
            _logger.Info($"Joining Room: #{roomId}");
            var result = await _client.CallAsync("joinRoom", TimeoutToken, roomId).ConfigureAwait(false);
            return result.ToObject<MethodResult>(JsonSerializer);
        }

        public async Task<MethodResult<RocketMessage>> SendMessageAsync(string text, string roomId)
        {
            _logger.Info($"Sending message to #{roomId}: {text}");
            var request = new
            {
                msg = text,
                rid = roomId,
                bot = IsBot
            };
            var result = await _client.CallAsync("sendMessage", TimeoutToken, request).ConfigureAwait(false);
            return result.ToObject<MethodResult<RocketMessage>>(JsonSerializer);
        }

        public async Task<MethodResult<RocketMessage>> SendCustomMessageAsync(Attachment attachment, string roomId)
        {
            var request = new
            {
                msg = "",
                rid = roomId,
                bot = IsBot,
                attachments = new[]
                {
                    attachment
                }
            };
            var result = await _client.CallAsync("sendMessage", TimeoutToken, request).ConfigureAwait(false);
            return result.ToObject<MethodResult<RocketMessage>>(JsonSerializer);
        }

        public async Task<MethodResult> UpdateMessageAsync(string messageId, string roomId, string newMessage)
        {
            _logger.Info($"Updating message {messageId}");
            var request = new
            {
                msg = newMessage,
                rid = roomId,
                bot = IsBot,
                _id = messageId
            };
            var result = await _client.CallAsync("updateMessage", TimeoutToken, request).ConfigureAwait(false);
            return result.ToObject<MethodResult>(JsonSerializer);
        }

        public async Task<MethodResult<LoadMessagesResult>> LoadMessagesAsync(string roomId, DateTime? end = null,
                                                                              int? limit = 20,
                                                                              string ls = null)
        {
            _logger.Info($"Loading messages from #{roomId}");

            var rawMessage = await _client.CallAsync("loadHistory", TimeoutToken, roomId, end, limit, ls).ConfigureAwait(false);
            var messageResult = rawMessage.ToObject<MethodResult<LoadMessagesResult>>(JsonSerializer);
            return messageResult;
        }

        public async Task<MethodResult<LoadMessagesResult>> SearchMessagesAsync(string query, string roomId,
                                                                                int limit = 100)
        {
            _logger.Info($"Searching for messages in #{roomId} using `{query}`.");

            var rawMessage = await _client.CallAsync("messageSearch", TimeoutToken, query, roomId, limit).ConfigureAwait(false);
            var messageResult = rawMessage.ToObject<MethodResult<LoadMessagesResult>>(JsonSerializer);
            return messageResult;
        }

        public async Task<MethodResult<StatisticsResult>> GetStatisticsAsync(bool refresh = false)
        {
            _logger.Info("Requesting statistics.");
            var results = await _client.CallAsync("getStatistics", TimeoutToken).ConfigureAwait(false);

            return results.ToObject<MethodResult<StatisticsResult>>(JsonSerializer);
        }

        public async Task<MethodResult<CreateRoomResult>> CreateChannelAsync(string roomName, IList<string> members = null)
        {
            _logger.Info($"Creating room {roomName}.");
            var results =
                await _client.CallAsync("createChannel", TimeoutToken, roomName, members ?? new List<string>()).ConfigureAwait(false);

            return results.ToObject<MethodResult<CreateRoomResult>>(JsonSerializer);
        }

        public async Task<MethodResult<CreateRoomResult>> HideRoomAsync(string roomId)
        {
            _logger.Info($"Hiding room {roomId}.");
            var results =
                await _client.CallAsync("hideRoom", TimeoutToken, roomId).ConfigureAwait(false);

            return results.ToObject<MethodResult<CreateRoomResult>>(JsonSerializer);
        }

        public async Task<MethodResult<int>> EraseRoomAsync(string roomId)
        {
            _logger.Info($"Deleting room {roomId}.");
            var results =
                await _client.CallAsync("eraseRoom", TimeoutToken, roomId).ConfigureAwait(false);

            return results.ToObject<MethodResult<int>>(JsonSerializer);
        }

        public async Task<MethodResult> ResetAvatarAsync()
        {
            var results = await _client.CallAsync("resetAvatar", TimeoutToken).ConfigureAwait(false);
            return results.ToObject<MethodResult>(JsonSerializer);
        }

        public async Task<MethodResult> SetAvatarFromUrlAsync(string url)
        {
            var results = await _client.CallAsync("setAvatarFromService", TimeoutToken, url, "", "url").ConfigureAwait(false);
            return results.ToObject<MethodResult>(JsonSerializer);
        }

        public async Task<MethodResult> SetAvatarFromImageStreamAsync(Stream sourceStream, string mimeType)
        {
            var base64 = EncodingHelper.ConvertToBase64(sourceStream);
            var end = $"data:{mimeType};base64,{base64}";
            var results = await _client.CallAsync("setAvatarFromService", TimeoutToken, end, mimeType, "upload").ConfigureAwait(false);
            return results.ToObject<MethodResult>(JsonSerializer);
        }

        public async Task<MethodResult<RocketMessage>> PinMessageAsync(RocketMessage message)
        {
            var results =
                await _client.CallAsync("pinMessage", TimeoutToken, message).ConfigureAwait(false);

            return results.ToObject<MethodResult<RocketMessage>>(JsonSerializer);
        }

        public async Task<MethodResult> UnpinMessageAsync(RocketMessage message)
        {
            var results =
                await _client.CallAsync("unpinMessage", TimeoutToken, message).ConfigureAwait(false);

            return results.ToObject<MethodResult>(JsonSerializer);
        }

        public async Task<MethodResult<int>> UploadFileAsync(string roomId)
        {
            var results =
                await _client.CallAsync("/rocketchat_uploads/insert", TimeoutToken, roomId).ConfigureAwait(false);

            return results.ToObject<MethodResult<int>>(JsonSerializer);
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

        [Obsolete("Use the property Rooms instead. This will be removed in version 2.")]
        public IEnumerable<Room> GetRooms()
        {
            var collection = GetRoomsCollection();
            if (collection == null)
            {
                yield break;
            }

            var rooms = collection.Items();
            foreach (var room in rooms.Select(x => x.Value))
            {
                yield return room;
            }
        }

        [CanBeNull]
        public TypedStreamCollection<Room> GetRoomsCollection()
        {
            IStreamCollection value;
            var results = _collectionDatabase.TryGetCollection("rocketchat_subscription", out value);
            if (!results)
            {
                return null;
            }

            var typedCollection = new TypedStreamCollection<Room>(value);
            return typedCollection;
        }

        [CanBeNull]
        public TypedStreamCollection<RoomInfo> GetRoomInfoCollection()
        {
            IStreamCollection value;
            var results = _collectionDatabase.TryGetCollection("rocketchat_room", out value);
            if (!results)
            {
                return null;
            }

            var typedCollection = new TypedStreamCollection<RoomInfo>(value);
            return typedCollection;
        }

        public async Task<MethodResult<IEnumerable<RoomInfo>>> GetAvailableRoomInfoCollection()
        {
            JObject result = await _client.CallAsync("rooms/get", CancellationToken.None, new object[] { 0 }).ConfigureAwait(false);
            return result.ToObject<MethodResult<IEnumerable<RoomInfo>>>(JsonSerializer);
        }

        public async Task<TypedStreamCollection<Room>> GetAvailableRoomsCollection()
        {
            JObject result = await _client.CallAsync("rooms/get", CancellationToken.None, new object[] { 0 }).ConfigureAwait(false);
            MethodResult<IEnumerable<RoomInfo>> roomMethodResult = result.ToObject<MethodResult<IEnumerable<RoomInfo>>>(JsonSerializer);
            throw new NotImplementedException();
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
            const int timeoutSeconds = 60;
            var source = new CancellationTokenSource();
            source.CancelAfter(TimeSpan.FromSeconds(timeoutSeconds));

            return source.Token;
        }
    }
}