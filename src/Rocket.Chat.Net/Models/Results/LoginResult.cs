namespace Rocket.Chat.Net.Models.Results
{
    using System;

    using Newtonsoft.Json;

    using Rocket.Chat.Net.Helpers;

    public class LoginResult
    {
        [JsonProperty(PropertyName = "id")]
        public string UserId { get; set; }

        public string Token { get; set; }

        [JsonConverter(typeof(MeteorDateSerializer))]
        public DateTime TokenExpires { get; set; }
    }
}