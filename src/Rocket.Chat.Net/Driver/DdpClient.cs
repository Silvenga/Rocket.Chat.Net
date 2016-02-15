namespace Rocket.Chat.Net.Driver
{
    using System;
    using System.Collections.Concurrent;
    using System.Threading;
    using System.Threading.Tasks;

    using Newtonsoft.Json;

    using Rocket.Chat.Net.Helpers;
    using Rocket.Chat.Net.Interfaces;
    using Rocket.Chat.Net.Models;

    using WebSocketSharp;

    public class DdpClient : IDisposable
    {
        private readonly ILogger _logger;
        private readonly WebSocket _socket;
        private readonly ConcurrentDictionary<string, dynamic> _messages = new ConcurrentDictionary<string, dynamic>();

        public string Url { get; set; }
        public bool UseSsl { get; set; }
        public string SessionId { get; set; }

        public event DataReceived DataReceivedRaw;

        public DdpClient(string baseUrl, bool useSsl, ILogger logger)
        {
            _logger = logger;
            Url = (useSsl ? "wss://" : "ws://") + baseUrl + "/websocket";
            UseSsl = useSsl;

            _socket = new WebSocket(Url);
            AttachEvents();
            _socket.Connect();
        }

        private void AttachEvents()
        {
            _socket.OnMessage += SocketOnMessage;
            _socket.OnClose += (sender, args) => _logger.Debug("CLOSE: " + args.Reason);
            _socket.OnError += (sender, args) => _logger.Info("ERROR: " + args.Message);
            _socket.OnOpen += (sender, args) => _logger.Debug("OPEN");
        }

        private void SocketOnMessage(object sender, MessageEventArgs messageEventArgs)
        {
            var json = messageEventArgs.Data;
            dynamic data = JsonConvert.DeserializeObject(json);
            _logger.Debug($"RECIEVED: {JsonConvert.SerializeObject(data, Formatting.Indented)}");

            if (DriverHelper.HasProperty(data, "msg"))
            {
                string type = data.msg;
                InternalHandle(type, data);
                OnDataReceived(type, data);
            }
        }

        private void InternalHandle(string type, dynamic data)
        {
            if (DriverHelper.HasProperty(data, "id"))
            {
                string id = data.id;
                _messages.TryAdd(id, data);
            }

            switch (type)
            {
                case "ping": // Required by spec
                    Pong(data);
                    break;
                case "connected":
                    SessionId = data.session;
                    _logger.Debug($"Connected via session {SessionId}.");
                    break;
            }
        }

        public async Task<dynamic> Ping()
        {
            var id = CreateId();
            var request = new
            {
                msg = "ping", id
            };

            await SendObject(request);

            var result = await WaitForIdAsync(id);

            return result;
        }

        private async Task Pong(dynamic data)
        {
            var request = new
            {
                msg = "pong", data.id
            };

            await SendObject(request);
        }

        public async Task ConnectAsync(string ddpVersion = "pre1")
        {
            var request = new
            {
                msg = "connect",
                version = ddpVersion,
                support = new[]
                {
                   ddpVersion
                }
            };

            await SendObject(request);
            await WaitForConnect();
        }

        public async Task<string> SubscribeAsync(string name, params dynamic[] args)
        {
            var id = CreateId();
            var request = new
            {
                msg = "sub",
                @params = args,
                name,
                id
            };

            await SendObject(request);
            return id;
        }

        public async Task<dynamic> CallAsync(string method, params object[] args)
        {
            var id = CreateId();
            var request = new
            {
                msg = "method",
                @params = args,
                method,
                id
            };

            await SendObject(request);
            var result = await WaitForIdAsync(id);

            return result;
        }

        private void OnDataReceived(string type, dynamic data)
        {
            DataReceivedRaw?.Invoke(type, data);
        }

        public void Dispose()
        {
            _socket.Close();
        }

        private async Task SendObject(dynamic data)
        {
            await Task.Run(() =>
            {
                var json = JsonConvert.SerializeObject(data, Formatting.Indented);
                _logger.Debug($"SEND: {json}");
                _socket.Send(json);
            });
        }

        private async Task<dynamic> WaitForIdAsync(string id)
        {
            var task = Task.Run(() =>
            {
                while (!_messages.ContainsKey(id))
                {
                    Thread.Sleep(10);
                }

                dynamic data;
                _messages.TryRemove(id, out data);
                return data;
            });

            return await task;
        }

        private async Task WaitForConnect()
        {
            await Task.Run(() =>
            {
                while (SessionId == null)
                {
                    Thread.Sleep(10);
                }
            });
        }

        private string CreateId()
        {
            return Guid.NewGuid().ToString("N");
        }
    }
}
