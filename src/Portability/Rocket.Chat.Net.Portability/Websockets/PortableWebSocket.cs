namespace Rocket.Chat.Net.Portability.Websockets
{
    using System;

    using Rocket.Chat.Net.Portability.Contracts;

#pragma warning disable CS0067
    public class PortableWebSocket : PortableWebSocketBase
    {
        public PortableWebSocket(string url) : base(url)
        {
            throw new NotImplementedException(
                "You are currently referencing a PCL skeleton - this should never happen. " +
                "This could either mean you missing a reference to Rocket.Chat.Net.Portability (NuGet should have installed it) " +
                "or Rocket.Chat.Net.Portability does not currently support your target platform. " +
                "In the latter case, contact the developer to see if the platform can be supported. ");
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