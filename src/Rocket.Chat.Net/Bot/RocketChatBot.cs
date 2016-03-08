namespace Rocket.Chat.Net.Bot
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Rocket.Chat.Net.Driver;
    using Rocket.Chat.Net.Interfaces;
    using Rocket.Chat.Net.Models;

    public class RocketChatBot
    {
        private readonly IRocketChatDriver _driver;
        private readonly ILogger _logger;
        private readonly List<IBotResponse> _botResponses = new List<IBotResponse>();

        public RocketChatBot(IRocketChatDriver driver, ILogger logger)
        {
            _driver = driver;
            _logger = logger;

            AttachEvents();
        }

        public RocketChatBot(string url, bool useSsl, ILogger logger)
        {
            _driver = new RocketChatDriver(url, useSsl, logger);
            _logger = logger;

            AttachEvents();
        }

        private void AttachEvents()
        {
            _driver.MessageReceived += DriverOnMessageReceived;
        }

        private void DriverOnMessageReceived(RocketMessage rocketMessage)
        {
            Task.Run(async () => // async this to prevent holding up the message loop
            {
                foreach (var botResponse in _botResponses)
                {
                    var hasResponse = false;
                    foreach (var response in botResponse.Response(rocketMessage))
                    {
                        hasResponse = true;
                        await _driver.SendMessageAsync(response.Message, response.RoomId);
                    }

                    if (hasResponse)
                    {
                        break;
                    }
                }
            });
        }

        public void AddResponse(IBotResponse botResponse)
        {
            _botResponses.Add(botResponse);
        }
    }
}
