namespace Rocket.Chat.Net.Tests.Helpers
{
    using System;

    using Newtonsoft.Json;

    using Rocket.Chat.Net.Helpers;

    public class MeteorDateSerializerModel
    {
        [JsonConverter(typeof(MeteorDateConverter))]
        public DateTime? DateTime { get; set; }
    }
}