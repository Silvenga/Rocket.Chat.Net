namespace Rocket.Chat.Net.Models
{
    using System.Collections.Generic;

    public class Reaction
    {
        public string Name { get; set; }

        public IList<string> Usernames { get; set; }
    }
}