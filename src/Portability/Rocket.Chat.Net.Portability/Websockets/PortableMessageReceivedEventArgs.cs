namespace Rocket.Chat.Net.Portability.Websockets
{
    public class PortableMessageReceivedEventArgs
    {
        public PortableMessageReceivedEventArgs(string message)
        {
            Message = message;
        }

        public string Message { get; set; }
    }
}