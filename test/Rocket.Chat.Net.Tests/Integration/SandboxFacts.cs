namespace Rocket.Chat.Net.Tests.Integration
{
    using System.Threading.Tasks;

    using Rocket.Chat.Net.Driver;
    using Rocket.Chat.Net.Tests.Helpers;

    using NLog;

    using Xunit;
    using Xunit.Abstractions;
    using Rocket.Chat.Net.Models.LoginOptions;

    [Trait("Category", "Sandbox")]
    public class SandboxFacts : DriverFactsBase
    {
        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
        private readonly ITestOutputHelper _helper;
        private readonly ILogger _xUnitLogger;

        public SandboxFacts(ITestOutputHelper helper) : base(helper)
        {
            _helper = helper;
            _xUnitLogger = NLog.LogManager.GetCurrentClassLogger();
        }

        [Fact(Skip = "skip")]
        public async Task Can_login()
        {
            //const string password = "SilverLight";

            //var driver = new RocketChatDriver("demo.rocket.chat", true, _xUnitLogger);

            var driver = new RocketChatDriver(Constants.RocketServer, false, _xUnitLogger);

            await driver.ConnectAsync();
            var loginResult = await driver.LoginWithEmailAsync(new EmailLoginOption() { Email = Constants.OneEmail, Password = Constants.OnePassword });

            var roomId = await driver.GetRoomIdAsync("GENERAL");

            await driver.JoinRoomAsync(roomId.Result);

            await driver.SubscribeToRoomAsync(roomId.Result);

            var messages = await driver.SendMessageAsync("", roomId.Result);

            driver.Dispose();
        }

        [Fact(Skip = "skip")]
        public async Task Send_attachments()
        {
            //const string password = "SilverLight";

            //var driver = new RocketChatDriver("demo.rocket.chat", true, _xUnitLogger);
            var userName = Constants.OneEmail;
            var password = Constants.OnePassword;

            var driver = new RocketChatDriver(Constants.RocketServer, false, _xUnitLogger);

            await driver.ConnectAsync();
            await driver.LoginWithEmailAsync(new EmailLoginOption() { Email = Constants.OneEmail, Password = Constants.OnePassword });

            var roomId = await driver.GetRoomIdAsync("GENERAL");

            //var a = await driver.SendCustomMessageAsync("test mesage", "name", roomId.Result);

            driver.Dispose();
        }

        [Fact(Skip = "skip")]
        public async Task Clean_up_rooms()
        {
            // Act
            await DefaultAccountLoginAsync();
            await CleanupRoomsAsync();

            // Assert
        }

        public override void Dispose()
        {
        }
    }
}