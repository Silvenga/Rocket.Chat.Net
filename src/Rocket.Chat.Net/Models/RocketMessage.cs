namespace Rocket.Chat.Net.Models
{
    using System;
    using System.Collections.Generic;

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
        public bool IsBotMentioned { get; set; }
        public bool IsFromMyself { get; set; }

        public bool WasEdited => EditedOn != null;

        /// <summary>
        /// uj: User joined
        /// </summary>
        public string Type { get; set; }
        public List<User> Mentions { get; set; }
        public List<User> Starred { get; set; }
    }
}
