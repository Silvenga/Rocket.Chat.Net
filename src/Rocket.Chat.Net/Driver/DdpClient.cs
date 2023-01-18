namespace Rocket.Chat.Net.Driver
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    using Rocket.Chat.Net.Interfaces;
    using Rocket.Chat.Net.Models;
    using WebSocket4Net;
    using SuperSocket.ClientEngine;
    using System.Security.Authentication;
    using NLog;

    public class DdpClient : IDdpClient
    {
        private readonly ILogger _logger;
        private readonly IWebSocketWrapper _socket;
        private readonly ConcurrentDictionary<string, JObject> _messages = new ConcurrentDictionary<string, JObject>();

        public string Url { get; }
        public string SessionId { get; private set; }
        public bool IsDisposed { get; private set; }

        public event DataReceived DataReceivedRaw;
        public event DdpReconnect DdpReconnect;

        public DdpClient(string baseUrl, bool useSsl, ILogger logger)
        {
            _logger = logger;

            var protocol = useSsl ? "wss" : "ws";
            Url = $"{protocol}://{baseUrl}/websocket";
                
            _socket = new WebSocketWrapper(new WebSocket(Url, sslProtocols : SslProtocols.Tls12 | SslProtocols.Tls11 | SslProtocols.Tls));
            AttachEvents();
        }

        public DdpClient(IWebSocketWrapper socket, ILogger logger)
        {
            _logger = logger;

            _socket = socket;
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
            if (SessionId != null && !IsDisposed)
            {
                // TODO: Fix reconnect
                try
                {
                    ConnectAsync(CancellationToken.None).Wait();
                }
                catch (Exception ex)
                {
                    _logger.Error(ex);
                }
            }
        }

        private void SocketOnError(object sender, ErrorEventArgs errorEventArgs)
        {
            _logger.Info("ERROR: " + errorEventArgs?.Exception?.Message);
        }

        private async void SocketOnOpened(object sender, EventArgs eventArgs)
        {
            _logger.Debug("OPEN");
            await SendConnectRequest().ConfigureAwait(false);
        }

        public async Task SendConnectRequest()
        {
            _logger.Debug("Sending connection request");
            const string ddpVersion = "1";
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

            await SendObjectAsync(request, CancellationToken.None).ConfigureAwait(false);
        }

        // TODO: Real time API implementieren
        private void SocketOnMessage(object sender, MessageReceivedEventArgs messageEventArgs)
        {
            var json = messageEventArgs.Message;
            var data = JObject.Parse(json);
            _logger.Debug($"RECEIVED: {JsonConvert.SerializeObject(data, Formatting.Indented)}");

            var isRocketMessage = data?["msg"] != null;
            if (isRocketMessage)
            {
                var type = data["msg"].ToObject<string>();
                InternalHandle(type, data);
                OnDataReceived(type, data);
            }
        }

        private void InternalHandle(string type, JObject data)
        {
            if (data["id"] != null)
            {
                var id = data["id"].ToObject<string>();
                _messages.TryAdd(id, data);
            }

            switch (type)
            {
                case "ping": // Required by spec
                    PongAsync(data).Wait();
                    break;
                case "connected":

                    if (SessionId != null)
                    {
                        OnDdpReconnect();
                    }
                    SessionId = data["session"].ToObject<string>();

                    _logger.Debug($"Connected via session {SessionId}.");
                    break;
                case "ready":
                    var subs = data["subs"];
                    var ids = subs?.ToObject<List<string>>();
                    var id = ids?.FirstOrDefault(); // TODO Handle collection?
                    if (id != null)
                    {
                        _messages.TryAdd(id, data);
                    }
                    break;
            }
        }

        public async Task PingAsync(CancellationToken token)
        {
            var id = CreateId();
            var request = new
            {
                msg = "ping",
                id
            };

            await SendObjectAsync(request, token).ConfigureAwait(false);
            await WaitForIdOrReadyAsync(id, token).ConfigureAwait(false);
        }

        private async Task PongAsync(JObject data)
        {
            var request = new
            {
                msg = "pong",
                id = data["id"]?.ToObject<string>()
            };

            await SendObjectAsync(request, CancellationToken.None).ConfigureAwait(false);
        }

        public async Task ConnectAsync(CancellationToken token)
        {
            _socket.Open();
            await WaitForConnectAsync(token).ConfigureAwait(false);
        }

        public async Task<string> SubscribeAsync(string name, CancellationToken token, params object[] args)
        {
            var id = CreateId();
            var request = new
            {
                msg = "sub",
                @params = args,
                name,
                id
            };

            await SendObjectAsync(request, token).ConfigureAwait(false);
            return id;
        }

        public async Task<string> SubscribeAndWaitAsync(string name, CancellationToken token, params object[] args)
        {
            var id = CreateId();
            var request = new
            {
                msg = "sub",
                @params = args,
                name,
                id
            };

            await SendObjectAsync(request, token).ConfigureAwait(false);
            JObject result = await WaitForIdOrReadyAsync(id, token).ConfigureAwait(false);
            return id;
        }

        public async Task UnsubscribeAsync(string id, CancellationToken token)
        {
            var request = new
            {
                msg = "unsub",
                id
            };

            await SendObjectAsync(request, token).ConfigureAwait(false);
            await WaitForIdOrReadyAsync(id, token).ConfigureAwait(false);
        }

        public async Task<JObject> CallAsync(string method, CancellationToken token, params object[] args)
        {
            var id = CreateId();
            var request = new
            {
                msg = "method",
                @params = args,
                method,
                id
            };

            await SendObjectAsync(request, token).ConfigureAwait(false);
            var result = await WaitForIdOrReadyAsync(id, token).ConfigureAwait(false);

            return result;
        }

        private void OnDataReceived(string type, JObject data)
        {
            DataReceivedRaw?.Invoke(type, data);
        }

        public void Dispose()
        {
            IsDisposed = true;
            SessionId = null;
            _socket.Close();
        }

        private async Task SendObjectAsync(object payload, CancellationToken token)
        {
            await Task.Run(() =>
            {
                var json = JsonConvert.SerializeObject(payload, Formatting.Indented);
                _logger.Debug($"SEND: {json}");
                _socket.Send(json);
            }, token).ConfigureAwait(false);
        }

        private async Task<JObject> WaitForIdOrReadyAsync(string id, CancellationToken token)
        {
            JObject data;
            while (!_messages.TryRemove(id, out data))
            {
                token.ThrowIfCancellationRequested();
                await Task.Delay(10, token).ConfigureAwait(false);
            }
            return data;
        }

        private async Task WaitForConnectAsync(CancellationToken token)
        {
            while (SessionId == null)
            {
                token.ThrowIfCancellationRequested();
                await Task.Delay(10, token).ConfigureAwait(false);
            }
        }

        private static string CreateId()
        {
            return Guid.NewGuid().ToString("N");
        }

        private void OnDdpReconnect()
        {
            DdpReconnect?.Invoke();
        }
    }
}