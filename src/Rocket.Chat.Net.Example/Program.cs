namespace Rocket.Chat.Net.Example
{
    using System;
    using System.Threading.Tasks;

    using Rocket.Chat.Net.Bot;
    using Rocket.Chat.Net.Driver;
    using Rocket.Chat.Net.Interfaces;
    using Rocket.Chat.Net.Models.Logins;

    public static class Program
    {
        public static void Main()
        {
            Task.Run(async () => await MainAsync());
            Console.ReadLine();
        }

        private static async Task MainAsync()
        {
            const string username = "m@silvenga.com";
            const string password = "silverlight";
            const string rocketServerUrl = "dev0:3000"; // just the host and port
            const bool useSsl = false; // Basically use ws or wss.

            // Basic logger
            ILogger logger = new ConsoleLogger();

            // Create the bot - an abstraction of the driver
            RocketChatBot bot = new RocketChatBot(rocketServerUrl, useSsl, logger);

            // Connect to Rocket.Chat
            await bot.Connect();

            // Login
            ILoginOption loginOption = new EmailLoginOption
            {
                Email = username,
                Password = password
            };
            await bot.Login(loginOption);

            // Start listening for messages
            await bot.Subscribe();

            // Add possible responses to be checked in order
            // This is not thead safe, FYI 
            IBotResponse giphyResponse = new GiphyResponse();
            bot.AddResponse(giphyResponse);

            // And that's it
            // Checkout GiphyResponse in the example project for more info.
        }
    }
}
