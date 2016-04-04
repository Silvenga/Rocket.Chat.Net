namespace Rocket.Chat.Net.Tests.Integration
{
    using System;
    using System.Threading.Tasks;

    using Rocket.Chat.Net.Driver;
    using Rocket.Chat.Net.Tests.Helpers;

    using Xunit;
    using Xunit.Abstractions;

    [Trait("Category", "Sandbox")]
    [Collection("Driver")]
    public class SandboxFacts : DriverFactsBase
    {
        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
        private readonly ITestOutputHelper _helper;
        private readonly XUnitLogger _xUnitLogger;

        public SandboxFacts(ITestOutputHelper helper) : base(helper)
        {
            _helper = helper;
            _xUnitLogger = new XUnitLogger(_helper);
        }

        [Fact(Skip = "")]
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

        [Fact(Skip = "")]
        public async Task Clean_up_rooms()
        {
            // Act
            await DefaultAccountLoginAsync();
            await CleanupRoomsAsync();

            // Assert
        }

        public void Dispose()
        {
            _xUnitLogger.Dispose();
        }
    }
}