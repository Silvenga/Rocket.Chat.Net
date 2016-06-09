namespace Rocket.Chat.Net.Portability.Websockets
{
    using System;

    public class PortableWebSocket
    {
        public PortableWebSocket(string url)
        {
            throw new NotImplementedException();
        }

        public event EventHandler<PortableMessageReceivedEventArgs> MessageReceived;
        public event EventHandler Closed;
        public event EventHandler<PortableErrorEventArgs> Error;
        public event EventHandler Opened;

        public void Open()
        {
            throw new NotImplementedException();
        }

        public void Close()
        {
            throw new NotImplementedException();
        }

        public void Send(string json)
        {
            throw new NotImplementedException();
        }
    }

    public class PortableMessageReceivedEventArgs
    {
        public PortableMessageReceivedEventArgs(string message)
        {
            Message = message;
        }

        public string Message { get; set; }
    }

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