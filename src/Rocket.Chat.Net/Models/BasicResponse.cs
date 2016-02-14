namespace Rocket.Chat.Net.Models
{
    public class BasicResponse
    {
        public string Message { get; set; }

        public string RoomId { get; set; }

        public BasicResponse(string message, string roomId)
        {
            Message = message;
            RoomId = roomId;
        }
    }
}
