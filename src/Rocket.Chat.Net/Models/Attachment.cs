namespace Rocket.Chat.Net.Models
{
    using System;

    using Newtonsoft.Json;

    using Rocket.Chat.Net.JsonConverters;

    public class Attachment
    {
        [JsonProperty(PropertyName = "text")]
        public string Text { get; set; }

        [JsonProperty(PropertyName = "author_name")]
        public string AuthorName { get; set; }

        [JsonProperty(PropertyName = "author_icon")]
        public string AuthorIcon { get; set; }

        [JsonProperty(PropertyName = "ts"), JsonConverter(typeof(MeteorDateConverter))]
        public DateTime? Timestamp { get; set; }
    }
}