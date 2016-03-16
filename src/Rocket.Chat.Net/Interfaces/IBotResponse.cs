namespace Rocket.Chat.Net.Interfaces
{
    using System.Collections.Generic;

    using Rocket.Chat.Net.Bot;
    using Rocket.Chat.Net.Models;

    public interface IBotResponse
    {
        IEnumerable<BasicResponse> Response(RocketMessage message, RocketChatBot caller);
    }
}
