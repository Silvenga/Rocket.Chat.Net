namespace Rocket.Chat.Net.Example
{
    using System;
    using System.Threading.Tasks;

    using Rocket.Chat.Net.Bot;
    using Rocket.Chat.Net.Driver;
    using Rocket.Chat.Net.Interfaces;

    public static class Program
    {
        private static ChatDriver _driver;

        public static void Main(string[] args)
        {
            Task.Run(async () => await MainAsync());
            Console.ReadLine();
        }

        private static async Task MainAsync()
        {
            const string userName = "m@silvenga.com";
            const string password = "silverlight";

            var logger = new ConsoleLogger();
            _driver = new ChatDriver("dev0:3000", false, logger);

            await _driver.ConnectAsync();
            await _driver.LoginWithPasswordAsync(userName, password);

            var roomResult = await _driver.GetRoomIdAsync("GENERAL");
            var roomId = roomResult.result.ToString();

            await _driver.JoinRoomAsync(roomId);
            await _driver.SubscribeToRoomAsync(roomId);

            var bot = new RocketChatBot(_driver, logger);

            bot.AddResponse(new GiphyResponse());
        }
    }
}
