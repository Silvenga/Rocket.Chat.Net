namespace Rocket.Chat.Net.Bot.Models
{
    using Rocket.Chat.Net.Bot.Interfaces;
    using Rocket.Chat.Net.Models;

    public class AttachmentResponse : IMessageResponse
    {
        /// <summary>
        /// Id of room to send this message to.
        /// </summary>
        public string RoomId { get; set; }

        public Attachment Attachment { get; set; }

        public AttachmentResponse(Attachment attachment, string roomId)
        {
            Attachment = attachment;
            RoomId = roomId;
        }
    }
}