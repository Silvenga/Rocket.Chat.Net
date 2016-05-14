namespace Rocket.Chat.Net.Driver
{
    using System;

    using Rocket.Chat.Net.Interfaces;

    using SuperSocket.ClientEngine;

    using WebSocket4Net;

    public class WebSocketWrapper : IWebSocketWrapper
    {
        private readonly WebSocket _socket;

        public WebSocketWrapper(WebSocket socket)
        {
            _socket = socket;
        }

        public event EventHandler<MessageReceivedEventArgs> MessageReceived
        {
            add { _socket.MessageReceived += value; }
            remove { _socket.MessageReceived -= value; }
        }

        public event EventHandler Closed
        {
            add { _socket.Closed += value; }
            remove { _socket.Closed -= value; }
        }

        public event EventHandler<ErrorEventArgs> Error
        {
            add { _socket.Error += value; }
            remove { _socket.Error -= value; }
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