namespace Rocket.Chat.Net.Bot.Interfaces
{
    using System.Collections.Generic;

    using Rocket.Chat.Net.Bot;
    using Rocket.Chat.Net.Bot.Models;
    using Rocket.Chat.Net.Models;

    public interface IBotResponse
    {
        bool CanRespond(ResponseContext context);

        IEnumerable<BasicResponse> GetResponse(ResponseContext context, RocketChatBot caller);
    }
}