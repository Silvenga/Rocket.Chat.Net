namespace Rocket.Chat.Net.Models
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Rocket message that was received.
    /// </summary>
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
    public class RocketMessage
    {
        /// <summary>
        /// Id of the rocket message.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Id of room the message was received in.
        /// </summary>
        public string RoomId { get; set; }

        /// <summary>
        /// The text contained in this message.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Whenever this message was sent by a bot.
        /// </summary>
        public bool IsBot { get; set; }

        /// <summary>
        /// User whom created the message.
        /// </summary>
        public User CreatedBy { get; set; }

        /// <summary>
        /// Timestamp when the message was sent.
        /// </summary>
        public DateTime? CreatedOn { get; set; }

        /// <summary>
        /// User whom last updated the message.
        /// </summary>
        public User EditedBy { get; set; }

        /// <summary>
        /// Timestamp when the message was sent.
        /// </summary>
        public DateTime? EditedOn { get; set; }

        /// <summary>
        /// Whenever this message mentions this bot.
        /// </summary>
        public bool IsBotMentioned { get; set; }

        /// <summary>
        /// Whenever this message is from myself.
        /// </summary>
        public bool IsFromMyself { get; set; }

        /// <summary>
        /// Whenever this message was edited by anyone.
        /// </summary>
        public bool WasEdited => EditedOn != null;

        /// <summary>
        /// The type of message.
        /// uj: User joined room
        /// ua: user invited to room
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// A list of users that this message mentions.
        /// </summary>
        public List<User> Mentions { get; set; }

        /// <summary>
        /// List of user Id's that this message was starred by.
        /// </summary>
        public List<User> Starred { get; set; }

        public override string ToString()
        {
            return $"{CreatedBy?.Username ?? "UnknownUser"}: {Message}";
        }
    }
}