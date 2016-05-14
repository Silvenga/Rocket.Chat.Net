namespace Rocket.Chat.Net.Bot.Helpers
{
    using Rocket.Chat.Net.Bot.Models;
    using Rocket.Chat.Net.Models;

    public static class BotHelper
    {
        public static BasicResponse CreateBasicReply(this RocketMessage message, string reply)
        {
            return new BasicResponse(reply, message.RoomId);
        }
    }
}