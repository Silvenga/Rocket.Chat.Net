namespace Rocket.Chat.Net.Models
{
    using System;
    using System.Collections.Generic;

    using Newtonsoft.Json;

    using Rocket.Chat.Net.JsonConverters;

    public class RoomInfo
    {
        [JsonProperty("_id")]
        public string Id { get; set; }

        [JsonProperty("ts"), JsonConverter(typeof(MeteorDateConverter))]
        public DateTime? Timestamp { get; set; }

        [JsonProperty("t")]
        public RoomType Type { get; set; }

        public string Name { get; set; }

        [JsonProperty("lm"), JsonConverter(typeof(MeteorDateConverter))]
        public DateTime? LastMessage { get; set; }

        [JsonProperty("msgs")]
        public int? MessageCount { get; set; }

        [JsonProperty("usernames")]
        public IList<string> Usersnames { get; set; }

        [JsonProperty(PropertyName = "u")]
        public User Owner { get; set; }
    }
}