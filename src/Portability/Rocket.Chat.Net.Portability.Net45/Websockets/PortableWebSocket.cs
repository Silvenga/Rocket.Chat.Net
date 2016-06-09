namespace Rocket.Chat.Net.Portability.Websockets
{
    using System;

    using WebSocket4Net;

    public class PortableWebSocket
    {
        private readonly WebSocket _socket;

        public PortableWebSocket(string url)
        {
            _socket = new WebSocket(url);
        }

        public event EventHandler<PortableMessageReceivedEventArgs> MessageReceived
        {
            add { _socket.MessageReceived += (sender, args) => value.Invoke(sender, new PortableMessageReceivedEventArgs(args.Message)); }
            remove { throw new NotImplementedException(); }
        }

        public event EventHandler Closed
        {
            add { _socket.Closed += value; }
            remove { _socket.Closed -= value; }
        }

        public event EventHandler<PortableErrorEventArgs> Error
        {
            add { _socket.Error += (sender, args) => value.Invoke(sender, new PortableErrorEventArgs(args.Exception)); }
            remove { throw new NotImplementedException(); }
        }

        public event EventHandler Opened
        {
            add { _socket.Opened += value; }
            remove { _socket.Opened -= value; }
        }

        public void Open()
        {
            _socket.Open();
        }

        public void Close()
        {
            _socket.Close();
        }

        public void Send(string json)
        {
            _socket.Send(json);
        }
    }
}