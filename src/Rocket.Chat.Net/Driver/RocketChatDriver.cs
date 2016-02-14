namespace Rocket.Chat.Net.Driver
{
    using System.Threading.Tasks;

    using Rocket.Chat.Net.Helpers;
    using Rocket.Chat.Net.Interfaces;
    using Rocket.Chat.Net.Models;

    public class RocketChatDriver : IRocketChatDriver
    {
        public const string MessageTopic = "stream-messages";
        public const int MessageSubscriptionLimit = 10;

        private readonly string _url;
        private readonly bool _useSsl;
        private readonly ILogger _logger;
        private DdpClient _client;

        public event MessageReceived MessageReceived;

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
        }

        private void ClientOnDataReceivedRaw(string type, dynamic data)
        {
            var isMessage = type == "added" && data.collection == MessageTopic && data.fields.args != null;
            if (isMessage)
            {
                var messageRaw = data.fields.args[1];
                var message = new RocketMessage
                {
                    Id = messageRaw["_id"],
                    RoomId = messageRaw.rid,
                    Message = messageRaw.msg.ToString().Trim(),
                    IsBot = messageRaw.bot != null && messageRaw.bot == true,
                    CreatedOn = DriverHelper.ParseDateTime(messageRaw.ts),
                    CreatedBy = DriverHelper.ParseUser(messageRaw.u),
                    EditedOn = DriverHelper.ParseDateTime(messageRaw.editedAt),
                    EditedBy = DriverHelper.ParseUser(messageRaw.editedBy)
                };

                var edit = message.IsEdit ? "(EDIT)" : "";
                _logger.Info($"Message from {message.CreatedBy.Username}@{message.RoomId}{edit}: {message.Message}");

                OnMessageReceived(message);
            }
        }

        public async Task ConnectAsync()
        {
            _logger.Info($"Connecting client to {_url}...");
            await _client.ConnectAsync();
        }

        public async Task SubscribeToRoomAsync(string roomId)
        {
            _logger.Info($"Subscribing to Room: #{roomId}");
            await _client.SubscribeAsync(MessageTopic, roomId, MessageSubscriptionLimit.ToString());
        }

        public async Task<dynamic> LoginWithEmailAsync(string email, string password)
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

            var result = await _client.CallAsync("login", request);
            return result;
        }

        public async Task<dynamic> LoginWithUsernameAsync(string username, string password)
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

            var result = await _client.CallAsync("login", request);
            return result;
        }

        public async Task LoginWithLdapAsync(string username, string password)
        {
            _logger.Info($"Logging in with user {username} using LDAP...");
            var request = new
            {
                username,
                password,
                ldapOptions = new { }
            };

            await _client.CallAsync("login", request);
        }

        public async Task<dynamic> GetRoomIdAsync(string roomId)
        {
            _logger.Info($"Looking up Room ID for: #{roomId}");
            return await _client.CallAsync("getRoomIdByNameOrId", roomId);
        }

        public async Task JoinRoomAsync(string roomId)
        {
            _logger.Info($"Joining Room: #{roomId}");
            await _client.CallAsync("joinRoom", roomId);
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
            return await _client.CallAsync("sendMessage", request);
        }

        protected void OnMessageReceived(RocketMessage rocketmessage)
        {
            MessageReceived?.Invoke(rocketmessage);
        }

        public void Dispose()
        {
            _client.Dispose();
        }
    }
}
