namespace Rocket.Chat.Net.Models.SubscriptionResults
{
    using System.Collections.Generic;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using Rocket.Chat.Net.Models.MethodResults;

    public class SubscriptionResult<T>
    {
        [JsonProperty(PropertyName = "msg")]
        public string ModificationType { get; set; }

        public string Collection { get; set; }

        public string Id { get; set; }

        public ErrorResult Error { get; set; } 

        public T Fields { get; set; }

        public IList<string> Cleared { get; set; }

        public IDictionary<string, JToken> ExtraData
        {
            get { return _extraData; }
            set { _extraData = value; }
        }

        [JsonExtensionData] private IDictionary<string, JToken> _extraData;
    }
}