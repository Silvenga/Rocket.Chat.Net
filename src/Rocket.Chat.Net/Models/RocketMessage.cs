namespace Rocket.Chat.Net.Models
{
    using System;

    public class RocketMessage
    {
        public string Id { get; set; }
        public string RoomId { get; set; }
        public string Message { get; set; }
        public bool IsBot { get; set; }
        public User CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public User EditedBy { get; set; }
        public DateTime? EditedOn { get; set; }

        public bool IsEdit => EditedOn != null;
    }
}
