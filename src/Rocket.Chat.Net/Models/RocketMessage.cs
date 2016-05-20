namespace Rocket.Chat.Net.Models
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    using JetBrains.Annotations;

    using Newtonsoft.Json;

    using Rocket.Chat.Net.Helpers;

    /// <summary>
    /// Rocket message that was received.
    /// </summary>
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
    [SuppressMessage("ReSharper", "CollectionNeverUpdated.Global")]
    public class RocketMessage
    {
        public RocketMessage()
        {
            Mentions = new List<User>();
            Starred = new List<User>();
            Attachments = new List<Attachment>();
        }

        /// <summary>
        /// Id of the rocket message.
        /// </summary>
        [JsonProperty(PropertyName = "_id")]
        public string Id { get; set; }

        /// <summary>
        /// Id of room the message was received in.
        /// </summary>
        [JsonProperty(PropertyName = "rid")]
        public string RoomId { get; set; }

        /// <summary>
        /// Room the message was recieved in.
        /// </summary>
        [JsonIgnore, CanBeNull]
        public Room Room { get; set; }

        /// <summary>
        /// The text contained in this message.
        /// </summary>
        [JsonProperty(PropertyName = "msg")]
        public string Message { get; set; }

        /// <summary>
        /// Whenever this message was sent by a bot.
        /// </summary>
        [JsonProperty(PropertyName = "bot")]
        public bool IsBot { get; set; }

        /// <summary>
        /// User whom created the message.
        /// </summary>
        [JsonProperty(PropertyName = "u")]
        public User CreatedBy { get; set; }

        /// <summary>
        /// Timestamp when the message was sent.
        /// </summary>
        [JsonProperty(PropertyName = "ts"), JsonConverter(typeof(MeteorDateConverter))]
        public DateTime? CreatedOn { get; set; }

        /// <summary>
        /// User whom last updated the message.
        /// </summary>
        [JsonProperty(PropertyName = "editedBy")]
        public User EditedBy { get; set; }

        /// <summary>
        /// Timestamp when the message was sent.
        /// </summary>
        [JsonProperty(PropertyName = "editedAt"), JsonConverter(typeof(MeteorDateConverter))]
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
        /// Whenever this message should be groupable
        /// </summary>
        [JsonProperty(PropertyName = "groupable")]
        public bool IsGroupable { get; set; }

        /// <summary>
        /// Whenever this message is pinned.
        /// </summary>
        [JsonProperty(PropertyName = "pinned")]
        public bool IsPinned { get; set; }

        /// <summary>
        /// Timestamp when the message was pinned.
        /// </summary>
        [JsonProperty(PropertyName = "pinnedAt"), JsonConverter(typeof(MeteorDateConverter))]
        public DateTime? PinnedAt { get; set; }

        /// <summary>
        /// User whom pinned the message.
        /// </summary>
        [JsonProperty(PropertyName = "pinnedBy")]
        public User PinnedBy { get; set; }

        /// <summary>
        /// Whenever this message was edited by anyone.
        /// </summary>
        public bool WasEdited => EditedOn != null;

        /// <summary>
        /// The type of message.
        /// uj: User joined room
        /// ua: user invited to room
        /// message_pinned: Message pinned
        /// Types: https://github.com/RocketChat/Rocket.Chat/blob/4d898024e23864fe96eaf776085c21fcd8991718/packages/rocketchat-lib/lib/MessageTypes.coffee
        /// </summary>
        [JsonProperty(PropertyName = "t")]
        public string Type { get; set; }

        /// <summary>
        /// A list of users that this message mentions.
        /// </summary>
        [JsonProperty(PropertyName = "mentions"), NotNull]
        public List<User> Mentions { get; set; }

        /// <summary>
        /// A list of reactions for this message.
        /// </summary>
        [JsonConverter(typeof(RocketReactionConverter))]
        public List<Reaction> Reactions { get; set; }

        /// <summary>
        /// List of user Id's that this message was starred by.
        /// WARNING: This may break in the future.
        /// </summary>
        [JsonProperty(PropertyName = "starred"), NotNull]
        public List<User> Starred { get; set; }

        /// <summary>
        /// List of attachments for this message.
        /// </summary>
        [JsonProperty(PropertyName = "attachments"), NotNull]
        public List<Attachment> Attachments { get; set; }

        /// <summary>
        /// TODO
        /// </summary>
        public List<RocketUrl> Urls { get; set; }

        public override string ToString()
        {
            return $"{CreatedBy?.Username ?? "UnknownUser"}: {Message}";
        }

        private bool Equals(RocketMessage other)
        {
            return string.Equals(Id, other.Id) && string.Equals(RoomId, other.RoomId) &&
                   string.Equals(Message, other.Message) && IsBot == other.IsBot && Equals(CreatedBy, other.CreatedBy) &&
                   CreatedOn.Equals(other.CreatedOn) && Equals(EditedBy, other.EditedBy) &&
                   EditedOn.Equals(other.EditedOn) && IsBotMentioned == other.IsBotMentioned &&
                   IsFromMyself == other.IsFromMyself && string.Equals(Type, other.Type) &&
                   Mentions.Equals(other.Mentions) && Starred.Equals(other.Starred);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            if (obj.GetType() != GetType())
                return false;
            return Equals((RocketMessage) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Id?.GetHashCode() ?? 0;
                hashCode = (hashCode * 397) ^ (RoomId?.GetHashCode() ?? 0);
                hashCode = (hashCode * 397) ^ (Message?.GetHashCode() ?? 0);
                hashCode = (hashCode * 397) ^ IsBot.GetHashCode();
                hashCode = (hashCode * 397) ^ (CreatedBy?.GetHashCode() ?? 0);
                hashCode = (hashCode * 397) ^ CreatedOn.GetHashCode();
                hashCode = (hashCode * 397) ^ (EditedBy?.GetHashCode() ?? 0);
                hashCode = (hashCode * 397) ^ EditedOn.GetHashCode();
                hashCode = (hashCode * 397) ^ IsBotMentioned.GetHashCode();
                hashCode = (hashCode * 397) ^ IsFromMyself.GetHashCode();
                hashCode = (hashCode * 397) ^ (Type?.GetHashCode() ?? 0);
                hashCode = (hashCode * 397) ^ Mentions.GetHashCode();
                hashCode = (hashCode * 397) ^ Starred.GetHashCode();
                return hashCode;
            }
        }
    }
}