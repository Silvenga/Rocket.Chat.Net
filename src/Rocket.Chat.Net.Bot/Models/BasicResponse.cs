namespace Rocket.Chat.Net.Bot.Models
{
    using Rocket.Chat.Net.Bot.Interfaces;

    /// <summary>
    /// A basic rocket response message.
    /// </summary>
    public class BasicResponse : IMessageResponse
    {
        /// <summary>
        /// Text of message.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Id of room to send this message to.
        /// </summary>
        public string RoomId { get; set; }

        public BasicResponse(string message, string roomId)
        {
            Message = message;
            RoomId = roomId;
        }
    }
}