namespace Rocket.Chat.Net.Models
{
    using System;
    using System.Collections.Generic;

    public class RoomWithInfo
    {
        /// <summary>
        /// Id of the room
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Last updated
        /// </summary>
        public DateTime? Timestamp { get; set; }

        /// <summary>
        /// The type of room
        /// </summary>
        public RoomType Type { get; set; }

        /// <summary>
        /// The name of the room
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Time in which the last message was sent
        /// </summary>
        public DateTime? LastMessage { get; set; }

        /// <summary>
        /// Number of messages in total
        /// </summary>
        public int? MessageCount { get; set; }

        /// <summary>
        /// Number of messages unread by the current user
        /// </summary>
        public int UnreadCount { get; set; }

        /// <summary>
        /// Users that belong in this group
        /// </summary>
        public IList<string> Usersnames { get; set; }

        /// <summary>
        /// The user whom either created this room or was given ownership
        /// </summary>
        public User Owner { get; set; }

        /// <summary>
        /// If the room is open by the user (e.g. can be seen from the UI)
        /// </summary>
        public bool IsOpen { get; set; }

        /// <summary>
        /// If there are any outstanding alerts for this room (someone tell me what this means please)
        /// </summary>
        public bool IsAlert { get; set; }

        /// <summary>
        /// Last time the current user view this room
        /// </summary>
        public DateTime? LastSeen { get; set; }
    }
}