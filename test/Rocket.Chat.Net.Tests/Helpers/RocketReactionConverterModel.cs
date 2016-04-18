namespace Rocket.Chat.Net.Tests.Helpers
{
    using System.Collections.Generic;

    using Newtonsoft.Json;

    using Rocket.Chat.Net.Helpers;
    using Rocket.Chat.Net.Models;

    public class RocketReactionConverterModel
    {
        [JsonConverter(typeof(RocketReactionConverter))]
        public IList<Reaction> Reactions { get; set; }
    }
}