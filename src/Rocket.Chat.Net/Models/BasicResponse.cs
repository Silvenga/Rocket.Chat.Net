namespace Rocket.Chat.Net.Models
{
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// A basic rocket response message.
    /// </summary>
    [SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Global")]
    public class BasicResponse
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