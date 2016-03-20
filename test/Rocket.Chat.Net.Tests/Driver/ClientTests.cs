namespace Rocket.Chat.Net.Tests.Driver
{
    using System;
    using System.Threading.Tasks;

    using Rocket.Chat.Net.Driver;
    using Rocket.Chat.Net.Interfaces;
    using Rocket.Chat.Net.Models.Logins;
    using Rocket.Chat.Net.Tests.Helpers;

    using Xunit;
    using Xunit.Abstractions;

    [Trait("Category", "Driver")]
    public class ClientTests : IDisposable
    {
        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
        private readonly ITestOutputHelper _helper;
        private readonly XUnitLogger _xUnitLogger;

        public ClientTests(ITestOutputHelper helper)
        {
            _helper = helper;
            _xUnitLogger = new XUnitLogger(_helper);
        }

        // TODO Write real tests

        [Fact]
        public async Task Can_login()
        {
            //const string password = "SilverLight";

            //var driver = new RocketChatDriver("demo.rocket.chat", true, _xUnitLogger);
            const string userName = "m@silvenga.com";
            const string password = "silverlight";

            var driver = new RocketChatDriver(Constants.RocketServer, false, _xUnitLogger);

            await driver.ConnectAsync();
            var loginResult = await driver.LoginWithEmailAsync(userName, password);

            var roomId = await driver.GetRoomIdAsync("GENERAL");

            await driver.JoinRoomAsync(roomId);

            await driver.SubscribeToRoomAsync(roomId);

            var messages = await driver.SearchMessagesAsync($"from:{driver.Username}", roomId);

            driver.Dispose();
        }

        [Fact]
        public async Task Should_do_something_on_bad_creds()
        {
            const string userName = "m@silvenga.com";
            const string password = "bad password";

            var driver = new RocketChatDriver("dev0:3000", false, _xUnitLogger);

            var emailLogin = new EmailLoginOption
            {
                Email = userName,
                Password = password
            };

            await driver.ConnectAsync();
            var loginResult = await driver.LoginWithEmailAsync(userName, password);

            Assert.True(loginResult.HasError);
            Assert.Equal(403, loginResult.ErrorData.Error);

            driver.Dispose();
        }

        [Fact]
        public async Task List_channels()
        {
            const string userName = "m@silvenga.com";
            const string password = "silverlight";

            IRocketChatDriver driver = new RocketChatDriver(Constants.RocketServer, false, _xUnitLogger);

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

            IRocketChatDriver driver = new RocketChatDriver(Constants.RocketServer, false, _xUnitLogger);

            await driver.ConnectAsync();
            var loginResult = await driver.LoginWithEmailAsync(userName, password);

            var result = await driver.LoadMessagesAsync("GENERAL");

            driver.Dispose();
        }

        public void Dispose()
        {
            _xUnitLogger.Dispose();
        }
    }
}