namespace Rocket.Chat.Net.Portability.Websockets
{
    using System;

    using Rocket.Chat.Net.Portability.Contracts;

#pragma warning disable CS0067
    public class PortableWebSocket : PortableWebSocketBase
    {
        public PortableWebSocket(string url) : base(url)
        {
        }

        public override event EventHandler<PortableMessageReceivedEventArgs> MessageReceived;
        public override event EventHandler Closed;
        public override event EventHandler<PortableErrorEventArgs> Error;
        public override event EventHandler Opened;

        public override void Open()
        {
            throw new NotImplementedException();
        }

        public override void Close()
        {
            throw new NotImplementedException();
        }

        public override void Send(string json)
        {
            throw new NotImplementedException();
        }
    }
#pragma warning restore CS0067
}