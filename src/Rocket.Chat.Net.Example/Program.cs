namespace Rocket.Chat.Net.Example
{
    using System;
    using System.Linq.Expressions;
    using System.Threading.Tasks;
    using NLog;
    using Rocket.Chat.Net.Bot;
    using Rocket.Chat.Net.Bot.Interfaces;
    using Rocket.Chat.Net.Interfaces;
    using Rocket.Chat.Net.Models.LoginOptions;

    public static class Program
    {
        public static void Main()
        {
            Task.Run(async () => await MainAsync());
            Console.ReadLine();
        }

        public static void SetUpLoggingConfiguration()
        {
            var config = new NLog.Config.LoggingConfiguration();
            config.AddTarget(new NLog.Targets.ConsoleTarget("Console"));
            config.AddRule(LogLevel.Debug, LogLevel.Fatal, "Console");
            NLog.LogManager.Configuration = config;
        }

        private static async Task MainAsync()
        {
            const string username = "theotest";
            const string password = "theotheo1234";
            const string rocketServerUrl = "chat.softbauware.de"; // just the host and port
            const bool useSsl = true; // Basically use ws or wss.

            SetUpLoggingConfiguration();

            Logger logger = NLog.LogManager.GetCurrentClassLogger();
            logger.Info("Logger initialized!");

            // Create the bot - an abstraction of the driver
            RocketChatBot bot = new RocketChatBot(rocketServerUrl, useSsl, logger);

            // Connect to Rocket.Chat
            await bot.ConnectAsync();

            // Login
            ILoginOption loginOption = new UsernameLoginOption()
            {
                Username = username,
                Password = password
            };
            try
            {
                await bot.LoginAsync(loginOption);
                Console.WriteLine("Logged in!");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            // Start listening for messages
            await bot.SubscribeAsync();

            await bot.Driver.SetUserPresence("online");

            // Add possible responses to be checked in order
            // This is not thead safe, FYI 
            IBotResponse giphyResponse = new GiphyResponse();
            IBotResponse helloWorldResponse = new HelloWorldResponse();
            bot.AddResponse(helloWorldResponse);
            bot.AddResponse(giphyResponse);


            // And that's it
            // Checkout GiphyResponse in the example project for more info.
        }
    }
}
