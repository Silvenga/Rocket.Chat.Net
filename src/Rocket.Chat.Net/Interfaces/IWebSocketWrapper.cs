namespace Rocket.Chat.Net.Interfaces
{
    using System;

    using Rocket.Chat.Net.Portability.Websockets;

    public interface IWebSocketWrapper
    {
        event EventHandler<PortableMessageReceivedEventArgs> MessageReceived;
        event EventHandler Closed;
        event EventHandler<PortableErrorEventArgs> Error;
        event EventHandler Opened;
        void Open();
        void Close();
        void Send(string json);
    }
}