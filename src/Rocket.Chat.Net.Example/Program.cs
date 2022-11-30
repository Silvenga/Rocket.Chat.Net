namespace Rocket.Chat.Net.Example
{
    using System;
    using System.Linq.Expressions;
    using System.Threading.Tasks;

    using Rocket.Chat.Net.Bot;
    using Rocket.Chat.Net.Bot.Interfaces;
    using Rocket.Chat.Net.Interfaces;
    using Rocket.Chat.Net.Loggers;
    using Rocket.Chat.Net.Models.LoginOptions;

    public static class Program
    {
        public static void Main()
        {
            Task.Run(async () => await MainAsync());
            Console.ReadLine();
        }

        private static async Task MainAsync()
        {
            const string username = "test@softbauware.de";
            const string password = "hallotheo123";
            const string rocketServerUrl = "softbauware.rocket.chat:443"; // just the host and port
            const bool useSsl = true; // Basically use ws or wss.
            
            // Create the bot - an abstraction of the driver
            RocketChatBot bot = new RocketChatBot(rocketServerUrl, useSsl);

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

            // Add possible responses to be checked in order
            // This is not thead safe, FYI 
            IBotResponse giphyResponse = new GiphyResponse();
            bot.AddResponse(giphyResponse);

            // And that's it
            // Checkout GiphyResponse in the example project for more info.
        }
    }
}
