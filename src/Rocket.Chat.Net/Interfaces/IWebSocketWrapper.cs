namespace Rocket.Chat.Net.Interfaces
{
    using System;

    using Newtonsoft.Json.Serialization;

    using Rocket.Chat.Net.Driver;
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