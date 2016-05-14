namespace Rocket.Chat.Net.Interfaces
{
    using System;

    using SuperSocket.ClientEngine;

    using WebSocket4Net;

    public interface IWebSocketWrapper
    {
        event EventHandler<MessageReceivedEventArgs> MessageReceived;
        event EventHandler Closed;
        event EventHandler<ErrorEventArgs> Error;
        event EventHandler Opened;
        void Open();
        void Close();
        void Send(string json);
    }
}