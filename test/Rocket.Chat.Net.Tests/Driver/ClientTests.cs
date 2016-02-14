namespace Rocket.Chat.Net.Tests.Driver
{
    using System.Threading.Tasks;

    using Rocket.Chat.Net.Driver;

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

            var driver = new ChatDriver("dev0:3000", false, new XUnitLogger(_helper));

            await driver.ConnectAsync();
            var loginResult = await driver.LoginWithPasswordAsync(userName, password);

            var roomResult = await driver.GetRoomIdAsync("GENERAL");
            var roomId = roomResult.result.ToString();

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

            var driver = new ChatDriver("dev0:3000", false, new XUnitLogger(_helper));

            await driver.ConnectAsync();
            var loginResult = await driver.LoginWithPasswordAsync(userName, password);

            Assert.Equal("403", loginResult.error.error.ToString());

            driver.Dispose();
        }
    }
}
