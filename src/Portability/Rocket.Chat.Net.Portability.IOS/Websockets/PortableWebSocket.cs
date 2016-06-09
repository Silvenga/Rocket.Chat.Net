namespace Rocket.Chat.Net.Portability.Websockets
{
    using System;

    using Rocket.Chat.Net.Portability.Contracts;

    using WebSocket4Net;

    public class PortableWebSocket : PortableWebSocketBase
    {
        private readonly WebSocket _socket;

        public PortableWebSocket(string url) : base(url)
        {
            _socket = new WebSocket(url);
        }

        public override event EventHandler<PortableMessageReceivedEventArgs> MessageReceived
        {
            add { _socket.MessageReceived += (sender, args) => value.Invoke(sender, new PortableMessageReceivedEventArgs(args.Message)); }
            remove { throw new NotImplementedException(); }
        }

        public override event EventHandler Closed
        {
            add { _socket.Closed += value; }
            remove { _socket.Closed -= value; }
        }

        public override event EventHandler<PortableErrorEventArgs> Error
        {
            add { _socket.Error += (sender, args) => value.Invoke(sender, new PortableErrorEventArgs(args.Exception)); }
            remove { throw new NotImplementedException(); }
        }

        public override event EventHandler Opened
        {
            add { _socket.Opened += value; }
            remove { _socket.Opened -= value; }
        }

        public override void Open()
        {
            _socket.Open();
        }

        public override void Close()
        {
            _socket.Close();
        }

        public override void Send(string json)
        {
            _socket.Send(json);
        }
    }
}