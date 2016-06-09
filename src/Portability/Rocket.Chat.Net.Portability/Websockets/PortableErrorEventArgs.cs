namespace Rocket.Chat.Net.Portability.Websockets
{
    using System;

    public class PortableErrorEventArgs
    {
        public PortableErrorEventArgs(Exception exception)
        {
            Exception = exception;
        }

        public PortableErrorEventArgs()
        {
        }

        public Exception Exception { get; set; }
    }
}