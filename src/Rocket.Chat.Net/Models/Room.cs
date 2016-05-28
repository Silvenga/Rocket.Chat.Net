namespace Rocket.Chat.Net.Models
{
    using System;
    using System.Runtime.Serialization;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    using Rocket.Chat.Net.JsonConverters;

    public class Room
    {
        [JsonProperty("open")]
        public bool IsOpen { get; set; }

        [JsonProperty("alert")]
        public bool IsAlert { get; set; }

        [JsonProperty("unread")]
        public int UnreadCount { get; set; }

        [JsonProperty("ts"), JsonConverter(typeof(MeteorDateConverter))]
        public DateTime Timestamp { get; set; }

        [JsonProperty("rid")]
        public string RoomId { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("t")]
        public RoomType Type { get; set; }

        [JsonProperty("ls"), JsonConverter(typeof(MeteorDateConverter))]
        public DateTime LastSeen { get; set; }
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum RoomType
    {
        [EnumMember(Value = "p")] PrivateGroup = 'p',
        [EnumMember(Value = "d")] DirectMessage = 'd',
        [EnumMember(Value = "c")] Channel = 'c'
    }
}