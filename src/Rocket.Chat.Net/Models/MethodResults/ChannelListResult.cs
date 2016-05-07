namespace Rocket.Chat.Net.Models.MethodResults
{
    using System.Collections.Generic;

    using Newtonsoft.Json;

    public class ChannelListResult
    {
        public List<Channel> Channels { get; set; }
    }

    public class Channel
    {
        [JsonProperty(PropertyName = "_id")]
        public string Id { get; set; }

        public string Name { get; set; }
    }
}