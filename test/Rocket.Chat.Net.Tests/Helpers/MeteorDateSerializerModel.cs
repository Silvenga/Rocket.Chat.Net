namespace Rocket.Chat.Net.Tests.Helpers
{
    using System;

    using Newtonsoft.Json;

    using Rocket.Chat.Net.Helpers;

    public class MeteorDateSerializerModel
    {
        [JsonConverter(typeof(MeteorDateSerializer))]
        public DateTime? DateTime { get; set; }
    }
}