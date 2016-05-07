namespace Rocket.Chat.Net.Tests.Integration
{
    using System;
    using System.Threading.Tasks;

    using FluentAssertions;

    using Ploeh.AutoFixture;

    using Rocket.Chat.Net.Tests.Helpers;

    using Xunit;
    using Xunit.Abstractions;

    [Trait("Category", "Driver")]
    public class LoginFacts : DriverFactsBase
    {
        public LoginFacts(ITestOutputHelper helper) : base(helper)
        {
            RocketChatDriver.ConnectAsync().Wait();
        }

        [Fact]
        public void Connecting_multiple_times_should_throw()
        {
            // Act
            Action action = () => RocketChatDriver.ConnectAsync().Wait();

            // Assert
            action.ShouldThrow<Exception>();
        }

        [Fact]
        public async Task Can_login_with_email()
        {
            // Act
            var loginResult = await RocketChatDriver.LoginWithEmailAsync(Constants.OneEmail, Constants.OnePassword);

            // Assert
            loginResult.Should().NotBeNull();
            loginResult.HasError.Should().BeFalse();
            loginResult.Result.Token.Should().NotBeNull();
        }

        [Fact]
        public async Task Can_login_with_username()
        {
            // Act
            var loginResult = await RocketChatDriver.LoginWithUsernameAsync(Constants.OneUsername, Constants.OnePassword);

            // Assert
            loginResult.Should().NotBeNull();
            loginResult.HasError.Should().BeFalse();
            loginResult.Result.Token.Should().NotBeNull();
        }

        [Fact]
        public async Task Can_login_with_token()
        {
            var tokenResult = await RocketChatDriver.LoginWithUsernameAsync(Constants.OneUsername, Constants.OnePassword);

            // Act
            var loginResult = await RocketChatDriver.LoginResumeAsync(tokenResult.Result.Token);

            // Assert
            loginResult.Should().NotBeNull();
            loginResult.HasError.Should().BeFalse();
            loginResult.Result.Token.Should().Be(tokenResult.Result.Token);
        }

        [Fact]
        public async Task Bad_login_should_have_error_data()
        {
            // Act
            var loginResult =
                await RocketChatDriver.LoginWithUsernameAsync(Constants.OneUsername, AutoFixture.Create<string>());

            // Assert
            loginResult.Should().NotBeNull();
            loginResult.HasError.Should().BeTrue();
            loginResult.Error.Message.Should().Be("Incorrect password [403]");
        }

        [Fact]
        public async Task When_logging_in_with_a_non_existing_user_return_error()
        {
            // Act
            var loginResult =
                await
                    RocketChatDriver.LoginWithUsernameAsync(AutoFixture.Create<string>(), AutoFixture.Create<string>());

            // Assert
            loginResult.Should().NotBeNull();
            loginResult.HasError.Should().BeTrue();
            loginResult.Error.Message.Should().Be("User not found [403]");
        }

        [Fact]
        public void Unknown_login_option_should_throw()
        {
            // Act
            Action action = () => RocketChatDriver.LoginAsync(new DummyLoginOption()).Wait();

            // Assert
            action.ShouldThrow<NotSupportedException>();
        }
    }
}