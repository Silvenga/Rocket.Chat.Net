namespace Rocket.Chat.Net.Bot
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Rocket.Chat.Net.Driver;
    using Rocket.Chat.Net.Interfaces;
    using Rocket.Chat.Net.Models;

    public class RocketChatBot
    {
        private readonly ILogger _logger;
        private readonly List<IBotResponse> _botResponses = new List<IBotResponse>();

        public IRocketChatDriver Driver { get; }

        public string LoginToken { get; private set; }

        public RocketChatBot(string url, bool useSsl, ILogger logger)
        {
            Driver = new RocketChatDriver(url, useSsl, logger);
            _logger = logger;

            Driver.MessageReceived += DriverOnMessageReceived;
            Driver.DdpReconnect += DriverOnDdpReconnect;
        }

        public async Task Connect()
        {
            await Driver.ConnectAsync();
        }

        public async Task Login(ILoginOption loginOption)
        {
            _logger.Info("Logging-in.");
            var result = await Driver.LoginAsync(loginOption);
            if (result.HasError)
            {
                throw new Exception($"Login failed: {result.ErrorData.Message}.");
            }

            LoginToken = result.Token;
        }

        public async Task Subscribe()
        {
            await Driver.SubscribeToRoomAsync();
        }

        public async Task Resume()
        {
            if (LoginToken == null)
            {
                throw new InvalidOperationException("Must have logged in first.");
            }

            _logger.Info($"Resuming session {LoginToken}.");
            var result = await Driver.LoginResumeAsync(LoginToken);
            if (result.HasError)
            {
                throw new Exception($"Resume failed: {result.ErrorData.Message}.");
            }
        }

        private void DriverOnMessageReceived(RocketMessage rocketMessage)
        {
            Task.Run(async () => // async this to prevent holding up the message loop
            {
                foreach (var botResponse in _botResponses)
                {
                    _logger.Info($"Trying response {botResponse.GetType()}.");
                    var hasResponse = false;
                    foreach (var response in botResponse.Response(rocketMessage))
                    {
                        hasResponse = true;
                        await Driver.SendMessageAsync(response.Message, response.RoomId);
                    }

                    if (hasResponse)
                    {
                        _logger.Info("Response succeeded.");
                        break;
                    }
                }
            });
        }

        private void DriverOnDdpReconnect()
        {
            _logger.Info($"Reconnect requested...");
            Task.Run(async () => await Resume());
        }

        public void AddResponse(IBotResponse botResponse)
        {
            _logger.Info($"Added response {botResponse.GetType()}.");
            _botResponses.Add(botResponse);
        }
    }
}
