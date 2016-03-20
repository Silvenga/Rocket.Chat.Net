namespace Rocket.Chat.Net.Tests.Driver
{
    using System;
    using System.Threading.Tasks;

    using FluentAssertions;

    using Rocket.Chat.Net.Driver;
    using Rocket.Chat.Net.Tests.Helpers;

    using Xunit;
    using Xunit.Abstractions;

    [Collection("Driver")]
    public class LoginFacts : IDisposable
    {
        private const string Email = "m@silvenga.com";
        private const string Username = "mark.lopez";
        private const string Password = "silverlight";

        private readonly RocketChatDriver _rocketChatDriver;
        private readonly XUnitLogger _xUnitLogger;

        public LoginFacts(ITestOutputHelper helper)
        {
            _xUnitLogger = new XUnitLogger(helper);
            _rocketChatDriver = new RocketChatDriver("dev0:3000", false, _xUnitLogger);
        }

        [Fact]
        public async Task Can_login_with_email()
        {
            var driver = _rocketChatDriver;

            await driver.ConnectAsync();
            var loginResult = await driver.LoginWithEmailAsync(Email, Password);

            loginResult.Should().NotBeNull();
            loginResult.HasError.Should().BeFalse();
            loginResult.Token.Should().NotBeNull();
        }
        
        [Fact]
        public async Task Can_login_with_username()
        {
            var driver = _rocketChatDriver;

            await driver.ConnectAsync();
            var loginResult = await driver.LoginWithUsernameAsync(Username, Password);

            loginResult.Should().NotBeNull();
            loginResult.HasError.Should().BeFalse();
            loginResult.Token.Should().NotBeNull();
        }

        [Fact]
        public async Task Can_login_with_token()
        {
            var driver = _rocketChatDriver;

            await driver.ConnectAsync();
            var tokenResult = await driver.LoginWithUsernameAsync(Username, Password);
            var loginResult = await driver.LoginResumeAsync(tokenResult.Token);

            loginResult.Should().NotBeNull();
            loginResult.HasError.Should().BeFalse();
            loginResult.Token.Should().Be(tokenResult.Token);
        }

        [Fact]
        public async Task Bad_login_should_have_error_data()
        {
            var driver = _rocketChatDriver;

            await driver.ConnectAsync();
            var loginResult = await driver.LoginWithUsernameAsync(Username, "Bad password");

            loginResult.Should().NotBeNull();
            loginResult.HasError.Should().BeTrue();
            loginResult.ErrorData.Message.Should().Be("Incorrect password [403]");
        }

        [Fact]
        public async Task Unknown_login_option_should_throw()
        {
            var driver = _rocketChatDriver;

            await driver.ConnectAsync();
            Action action = () => driver.LoginAsync(new DummyLoginOption()).Wait();

            action.ShouldThrow<NotSupportedException>();
        }

        public void Dispose()
        {
            _xUnitLogger.Dispose();
            _rocketChatDriver.Dispose();
        }
    }
}
