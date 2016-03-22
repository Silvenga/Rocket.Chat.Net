namespace Rocket.Chat.Net.Models.Results
{
    using Newtonsoft.Json;

    public abstract class BaseResult
    {
        [JsonProperty(PropertyName = "error")]
        public ErrorData ErrorData { get; set; }

        public bool HasError => ErrorData != null;
    }
}