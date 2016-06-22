namespace Rocket.Chat.Net.Models
{
    using System;
    using System.Collections.Generic;

    public class RoomWithInfo
    {
        public string Id { get; set; }

        public DateTime? Timestamp { get; set; }

        public RoomType Type { get; set; }

        public string Name { get; set; }

        public DateTime? LastMessage { get; set; }

        public int? MessageCount { get; set; }

        public IList<string> Usersnames { get; set; }

        public User Owner { get; set; }

        public bool IsOpen { get; set; }

        public bool IsAlert { get; set; }

        public int UnreadCount { get; set; }

        public DateTime? LastSeen { get; set; }
    }
}