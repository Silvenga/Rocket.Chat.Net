namespace Rocket.Chat.Net.Models.SubscriptionResults
{
    using System.Collections.Generic;

    using Newtonsoft.Json;

    public class SubscriptionResult<T>
    {
        [JsonProperty(PropertyName = "msg")]
        public string ModificationType { get; set; }

        public string Collection { get; set; }

        public string Id { get; set; }

        public T Fields { get; set; }

        public IList<string> Cleared { get; set; }
    }
}