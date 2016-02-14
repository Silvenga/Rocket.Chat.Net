namespace Rocket.Chat.Net.Example
{
    using System;
    using System.Threading.Tasks;

    using Rocket.Chat.Net.Bot;
    using Rocket.Chat.Net.Driver;
    using Rocket.Chat.Net.Interfaces;

    public static class Program
    {
        private static IRocketChatDriver _driver;

        public static void Main()
        {
            Task.Run(async () => await MainAsync());
            Console.ReadLine();
        }

        private static async Task MainAsync()
        {
            const string userName = "m@silvenga.com";
            const string password = "silverlight";

            var logger = new ConsoleLogger();
            _driver = new RocketChatDriver("dev0:3000", false, logger);

            // Request connection to Rocket.Chat
            await _driver.ConnectAsync();

            // Login with a email address (opposed to logging in with LDAP or Username)
            await _driver.LoginWithEmailAsync(userName, password);

            var roomResult = await _driver.GetRoomIdAsync("GENERAL");
            var roomId = roomResult.result.ToString();

            await _driver.JoinRoomAsync(roomId);

            // Start listening for messages
            await _driver.SubscribeToRoomAsync(roomId);

            // Create the bot - an abstraction of the driver
            var bot = new RocketChatBot(_driver, logger);

            // Add a possible response
            // This is not thead safe, FYI 
            bot.AddResponse(new GiphyResponse());
        }
    }
}
