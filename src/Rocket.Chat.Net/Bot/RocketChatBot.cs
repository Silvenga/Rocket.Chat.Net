namespace Rocket.Chat.Net.Bot
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Rocket.Chat.Net.Driver;
    using Rocket.Chat.Net.Interfaces;
    using Rocket.Chat.Net.Models;

    public class RocketChatBot : IDisposable
    {
        private readonly ILogger _logger;
        private readonly List<IBotResponse> _botResponses = new List<IBotResponse>();

        // ReSharper disable once MemberCanBePrivate.Global
        public IRocketChatDriver Driver { get; }

        // ReSharper disable once MemberCanBePrivate.Global
        public string LoginToken { get; private set; }

        public RocketChatBot(string url, bool useSsl, ILogger logger)
        {
            Driver = new RocketChatDriver(url, useSsl, logger);
            _logger = logger;

            Driver.MessageReceived += DriverOnMessageReceived;
            Driver.DdpReconnect += DriverOnDdpReconnect;
        }

        public async Task ConnectAsync()
        {
            await Driver.ConnectAsync();
        }

        public async Task LoginAsync(ILoginOption loginOption)
        {
            _logger.Info("Logging-in.");
            var result = await Driver.LoginAsync(loginOption);
            if (result.HasError)
            {
                throw new Exception($"Login failed: {result.ErrorData.Message}.");
            }

            LoginToken = result.Result.Token;
        }

        public async Task SubscribeAsync()
        {
            await Driver.SubscribeToRoomAsync();
        }

        public async Task ResumeAsync()
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
                    try
                    {
                        _logger.Debug($"Trying response {botResponse.GetType()}.");
                        var hasResponse = false;
                        foreach (var response in botResponse.Response(rocketMessage, this))
                        {
                            hasResponse = true;
                            await Driver.SendMessageAsync(response.Message, response.RoomId);
                        }

                        if (hasResponse)
                        {
                            _logger.Debug("Response succeeded.");
                            break;
                        }
                    }
                    catch (Exception e)
                    {
                        _logger.Info($"ERROR: {e}");
                    }
                }
            });
        }

        private void DriverOnDdpReconnect()
        {
            _logger.Info("Reconnect requested...");
            Task.Run(async () => await ResumeAsync());
        }

        public void AddResponse(IBotResponse botResponse)
        {
            _logger.Info($"Added response {botResponse.GetType()}.");
            _botResponses.Add(botResponse);
        }

        public void Dispose()
        {
            Driver.Dispose();
        }
    }
}