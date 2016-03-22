namespace Rocket.Chat.Net.Models
{
    using System;
    using System.Collections.Generic;

    using Newtonsoft.Json;

    using Rocket.Chat.Net.Helpers;

    public class FullUser
    {
        public string Id { get; set; }

        [JsonConverter(typeof(MeteorDateSerializer))]
        public DateTime CreatedAt { get; set; }

        public string Username { get; set; }

        public List<Email> Emails { get; set; }

        public string Status { get; set; }

        public bool Active { get; set; }

        public List<string> Roles { get; set; }
    }
}