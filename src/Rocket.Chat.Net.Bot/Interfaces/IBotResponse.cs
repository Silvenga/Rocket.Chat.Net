namespace Rocket.Chat.Net.Bot.Interfaces
{
    using System.Collections.Generic;

    using Rocket.Chat.Net.Bot.Models;

    public interface IBotResponse
    {
        bool CanRespond(ResponseContext context);

        IEnumerable<IMessageResponse> GetResponse(ResponseContext context, RocketChatBot caller);
    }
}