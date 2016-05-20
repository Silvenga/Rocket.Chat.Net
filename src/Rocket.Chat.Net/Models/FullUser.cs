namespace Rocket.Chat.Net.Models
{
    using System;
    using System.Collections.Generic;

    using Newtonsoft.Json;

    using Rocket.Chat.Net.Helpers;
    using Rocket.Chat.Net.JsonConverters;

    public class FullUser
    {
        public string Id { get; set; }

        [JsonConverter(typeof(MeteorDateConverter))]
        public DateTime CreatedAt { get; set; }

        public string Username { get; set; }

        public List<Email> Emails { get; set; }

        public string Status { get; set; }

        public bool Active { get; set; }

        public List<string> Roles { get; set; }
    }
}