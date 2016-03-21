namespace Rocket.Chat.Net.Tests.Driver
{
    using System;
    using System.Threading.Tasks;

    using Rocket.Chat.Net.Driver;
    using Rocket.Chat.Net.Models.Logins;
    using Rocket.Chat.Net.Tests.Helpers;

    using Xunit;
    using Xunit.Abstractions;

    [Trait("Category", "Sandbox")]
    public class SandboxFacts : IDisposable
    {
        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
        private readonly ITestOutputHelper _helper;
        private readonly XUnitLogger _xUnitLogger;

        public SandboxFacts(ITestOutputHelper helper)
        {
            _helper = helper;
            _xUnitLogger = new XUnitLogger(_helper);
        }
        
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

        public void Dispose()
        {
            _xUnitLogger.Dispose();
        }
    }
}