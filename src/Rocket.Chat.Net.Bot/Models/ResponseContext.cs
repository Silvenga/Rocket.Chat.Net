namespace Rocket.Chat.Net.Bot.Models
{
    using Rocket.Chat.Net.Models;

    public class ResponseContext
    {
        public bool BotHasResponded { get; set; }
        public string BotUserId { get; set; }
        public string BotUserName { get; set; }
        public RocketMessage Message { get; set; }
    }
}