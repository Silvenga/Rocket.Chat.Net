namespace Rocket.Chat.Net.Helpers
{
    using Rocket.Chat.Net.Models;

    public static class BotHelper
    {
        public static BasicResponse CreateBasicReply(this RocketMessage message, string reply)
        {
            return new BasicResponse(reply, message.RoomId);
        }
    }
}
