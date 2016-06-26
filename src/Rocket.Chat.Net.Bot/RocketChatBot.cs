namespace Rocket.Chat.Net.Bot
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Rocket.Chat.Net.Bot.Interfaces;
    using Rocket.Chat.Net.Bot.Models;
    using Rocket.Chat.Net.Driver;
    using Rocket.Chat.Net.Interfaces;
    using Rocket.Chat.Net.Loggers;
    using Rocket.Chat.Net.Models;

    public class RocketChatBot : IDisposable
    {
        private readonly ILogger _logger;
        private readonly List<IBotResponse> _botResponses = new List<IBotResponse>();

        public IRocketChatDriver Driver { get; }

        public string LoginToken { get; private set; }

        public RocketChatBot(IRocketChatDriver driver, ILogger logger)
        {
            Driver = driver;
            _logger = logger ?? new DummyLogger();

            Driver.MessageReceived += DriverOnMessageReceived;
            Driver.DdpReconnect += DriverOnDdpReconnect;
        }

        public RocketChatBot(string url, bool useSsl, ILogger logger = null)
            : this(new RocketChatDriver(url, useSsl, logger), logger)
        {
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
                throw new Exception($"Login failed: {result.Error.Message}.");
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
                throw new Exception($"Resume failed: {result.Error.Message}.");
            }

            LoginToken = result.Result.Token;
        }

        public async Task LogoutOtherClientsAsync()
        {
            if (LoginToken == null)
            {
                throw new InvalidOperationException("Must have logged in first.");
            }

            _logger.Info($"Getting new token {LoginToken}.");
            var newToken = await Driver.GetNewTokenAsync();
            if (newToken.HasError)
            {
                throw new Exception($"Resume failed: {newToken.Error.Message}.");
            }

            _logger.Info($"Logging out all other users {LoginToken}.");
            var result = await Driver.RemoveOtherTokensAsync();
            if (result.HasError)
            {
                throw new Exception($"Resume failed: {result.Error.Message}.");
            }

            LoginToken = newToken.Result.Token;
        }

        public void AddResponse(IBotResponse botResponse)
        {
            _logger.Info($"Added response {botResponse.GetType()}.");
            _botResponses.Add(botResponse);
        }

        private void DriverOnMessageReceived(RocketMessage rocketMessage)
        {
            var context = new ResponseContext
            {
                Message = rocketMessage,
                BotHasResponded = false,
                BotUserId = Driver.UserId,
                BotUserName = Driver.Username
            };

            Task.Run(async () => // async this to prevent holding up the message loop, I will handle exceptions.
            {
                foreach (var botResponse in GetValidResponses(context, _botResponses))
                {
                    try
                    {
                        _logger.Debug($"Trying response {botResponse.GetType()}.");
                        var hasResponse = false;
                        foreach (var response in botResponse.GetResponse(context, this))
                        {
                            hasResponse = true;
                            await SendMessageAsync(response);
                        }

                        if (hasResponse)
                        {
                            _logger.Debug("Response succeeded.");
                        }
                    }
                    catch (Exception e)
                    {
                        _logger.Info($"ERROR: {e}");
                    }
                }
            });
        }

        private async Task SendMessageAsync(IMessageResponse response)
        {
            var attachmentMessage = response as AttachmentResponse;
            var basicMessage = response as BasicResponse;

            if (attachmentMessage != null)
            {
                await Driver.SendCustomMessageAsync(attachmentMessage.Attachment, attachmentMessage.RoomId);
            }
            else if (basicMessage != null)
            {
                await Driver.SendMessageAsync(basicMessage.Message, basicMessage.RoomId);
            }
            else
            {
                throw new InvalidOperationException(
                    $"The result of {nameof(IBotResponse.GetResponse)} is either null or not of a supported type.");
            }
        }

        private IEnumerable<IBotResponse> GetValidResponses(ResponseContext context, IEnumerable<IBotResponse> possibleResponses)
        {
            foreach (var response in possibleResponses)
            {
                var canRespond = response.CanRespond(context);
                if (canRespond)
                {
                    context.BotHasResponded = true;
                    yield return response;
                }
            }
        }

        private void DriverOnDdpReconnect()
        {
            _logger.Info("Reconnect requested...");
            if (LoginToken != null)
            {
                ResumeAsync().Wait();
            }
        }

        public void Dispose()
        {
            Driver.Dispose();
        }
    }
}