using Rocket.Chat.Net.Bot;
using Rocket.Chat.Net.Bot.Interfaces;
using Rocket.Chat.Net.Bot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rocket.Chat.Net.Example
{
    public class HelloWorldResponse : IBotResponse
    {
        public bool CanRespond(ResponseContext context)
        {
            return ! context.Message.IsFromMyself  && context.Message.Message.ToLower().StartsWith("hello");
        }

        public IEnumerable<IMessageResponse> GetResponse(ResponseContext context, RocketChatBot caller)
        {
            var message = context.Message;
            yield return new BasicResponse("Hello world!", context.Message.RoomId);
        }
    }
}
