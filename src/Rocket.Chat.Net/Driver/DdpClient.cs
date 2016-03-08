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

    using SuperSocket.ClientEngine;

    using WebSocket4Net;

    public class DdpClient : IDisposable
    {
        private readonly ILogger _logger;
        private readonly WebSocket _socket;
        private readonly ConcurrentDictionary<string, dynamic> _messages = new ConcurrentDictionary<string, dynamic>();

        public string Url { get; }
        public string SessionId { get; private set; }

        public event DataReceived DataReceivedRaw;
        public event DdpReconnect DdpReconnect;

        public DdpClient(string baseUrl, bool useSsl, ILogger logger)
        {
            _logger = logger;
            Url = (useSsl ? "wss://" : "ws://") + baseUrl + "/websocket";

            _socket = new WebSocket(Url);
            AttachEvents();
        }

        private void AttachEvents()
        {
            _socket.MessageReceived += SocketOnMessage;
            _socket.Closed += SocketOnClosed;
            _socket.Error += SocketOnError;
            _socket.Opened += SocketOnOpened;
        }

        private void SocketOnClosed(object sender, EventArgs eventArgs)
        {
            _logger.Debug("CLOSE");
            if(SessionId != null)
            {
                ConnectAsync(CancellationToken.None).Wait();
            }
        }

        private void SocketOnError(object sender, ErrorEventArgs errorEventArgs)
        {
            _logger.Info("ERROR: " + errorEventArgs?.Exception?.Message);
        }

        private void SocketOnOpened(object sender, EventArgs eventArgs)
        {
            _logger.Debug("OPEN");

            _logger.Debug("Sending connection request");
            const string ddpVersion = "pre1";
            var request = new
            {
                msg = "connect",
                version = ddpVersion,
                session = SessionId, // Although, it doesn't appear that RC handles resuming sessions
                support = new[]
                {
                    ddpVersion
                }
            };

            SendObjectAsync(request, CancellationToken.None).Wait();
        }

        private void SocketOnMessage(object sender, MessageReceivedEventArgs messageEventArgs)
        {
            var json = messageEventArgs.Message;
            dynamic data = JsonConvert.DeserializeObject(json);
            _logger.Debug($"RECIEVED: {JsonConvert.SerializeObject(data, Formatting.Indented)}");

            var isRocketMessage = DriverHelper.HasProperty(data, "msg");
            if(isRocketMessage)
            {
                string type = data.msg;
                InternalHandle(type, data);
                OnDataReceived(type, data);
            }
        }

        private void InternalHandle(string type, dynamic data)
        {
            if(DriverHelper.HasProperty(data, "id"))
            {
                string id = data.id;
                _messages.TryAdd(id, data);
            }

            switch(type)
            {
                case "ping": // Required by spec
                    Pong(data);
                    break;
                case "connected":

                    if(SessionId != null)
                    {
                        OnDdpReconnect();
                    }
                    SessionId = data.session;

                    _logger.Debug($"Connected via session {SessionId}.");
                    break;
            }
        }

        public async Task<dynamic> Ping(CancellationToken token)
        {
            var id = CreateId();
            var request = new
            {
                msg = "ping",
                id
            };

            await SendObjectAsync(request, token);

            var result = await WaitForIdAsync(id, token);

            return result;
        }

        private async Task Pong(dynamic data)
        {
            var request = new
            {
                msg = "pong",
                data.id
            };

            await SendObjectAsync(request, CancellationToken.None);
        }

        public async Task ConnectAsync(CancellationToken token)
        {
            _socket.Open();
            await WaitForConnectAsync(token);
        }

        public async Task<string> SubscribeAsync(string name, CancellationToken token, params dynamic[] args)
        {
            var id = CreateId();
            var request = new
            {
                msg = "sub",
                @params = args,
                name,
                id
            };

            await SendObjectAsync(request, token);
            return id;
        }

        public async Task<dynamic> CallAsync(string method, CancellationToken token, params object[] args)
        {
            var id = CreateId();
            var request = new
            {
                msg = "method",
                @params = args,
                method,
                id
            };

            await SendObjectAsync(request, token);
            var result = await WaitForIdAsync(id, token);

            return result;
        }

        private void OnDataReceived(string type, dynamic data)
        {
            DataReceivedRaw?.Invoke(type, data);
        }

        public void Dispose()
        {
            SessionId = null;
            _socket.Close();
        }

        private async Task SendObjectAsync(dynamic data, CancellationToken token)
        {
            await Task.Run(() =>
            {
                var json = JsonConvert.SerializeObject(data, Formatting.Indented);
                _logger.Debug($"SEND: {json}");
                _socket.Send(json);
            }, token);
        }

        private async Task<dynamic> WaitForIdAsync(string id, CancellationToken token)
        {
            var task = Task.Run(() =>
            {
                while(!_messages.ContainsKey(id))
                {
                    token.ThrowIfCancellationRequested();
                    Thread.Sleep(10);
                }

                dynamic data;
                _messages.TryRemove(id, out data);
                return data;
            }, token);

            return await task;
        }

        private async Task WaitForConnectAsync(CancellationToken token)
        {
            await Task.Run(() =>
            {
                while(SessionId == null)
                {
                    token.ThrowIfCancellationRequested();
                    Thread.Sleep(10);
                }
            }, token);
        }

        private static string CreateId()
        {
            return Guid.NewGuid().ToString("N");
        }

        protected virtual void OnDdpReconnect()
        {
            DdpReconnect?.Invoke();
        }
    }
}