namespace Rocket.Chat.Net.Models.MethodResults
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    public class CollectionResult
    {
        [JsonProperty(PropertyName = "collection")]
        public string Name { get; set; }

        public string Id { get; set; }

        public JObject Fields { get; set; }
    }
}