namespace Rocket.Chat.Net.Tests.Driver
{
    using System.Threading.Tasks;

    using Rocket.Chat.Net.Driver;
    using Rocket.Chat.Net.Interfaces;

    using Xunit;
    using Xunit.Abstractions;

    public class ClientTests
    {
        private readonly ITestOutputHelper _helper;

        public ClientTests(ITestOutputHelper helper)
        {
            _helper = helper;
        }

        // TODO Write real tests

        [Fact]
        public async Task Can_login()
        {
            const string userName = "m@silvenga.com";
            const string password = "silverlight";

            var driver = new RocketChatDriver("dev0:3000", false, new XUnitLogger(_helper));

            await driver.ConnectAsync();
            var loginResult = await driver.LoginWithEmailAsync(userName, password);

            var roomId = await driver.GetRoomIdAsync("GENERAL");

            await driver.JoinRoomAsync(roomId);

            await driver.SubscribeToRoomAsync(roomId);

            await driver.SendMessageAsync("hello world", roomId);

            driver.Dispose();
        }

        [Fact]
        public async Task Should_do_something_on_bad_creds()
        {
            const string userName = "m@silvenga.com";
            const string password = "bad password";

            var driver = new RocketChatDriver("dev0:3000", false, new XUnitLogger(_helper));

            await driver.ConnectAsync();
            var loginResult = await driver.LoginWithEmailAsync(userName, password);

            Assert.Equal("403", loginResult.error.error.ToString());

            driver.Dispose();
        }

        [Fact]
        public async Task List_channels()
        {
            const string userName = "m@silvenga.com";
            const string password = "silverlight";

            IRocketChatDriver driver = new RocketChatDriver("dev0:3000", false, new XUnitLogger(_helper));

            await driver.ConnectAsync();
            var loginResult = await driver.LoginWithEmailAsync(userName, password);

            var result = await driver.ChannelListAsync();

            driver.Dispose();
        }

        [Fact]
        public async Task Message_history()
        {
            const string userName = "m@silvenga.com";
            const string password = "silverlight";

            IRocketChatDriver driver = new RocketChatDriver("dev0:3000", false, new XUnitLogger(_helper));

            await driver.ConnectAsync();
            var loginResult = await driver.LoginWithEmailAsync(userName, password);

            var result = await driver.LoadMessagesAsync("GENERAL");

            driver.Dispose();
        }
    }
}
