namespace Rocket.Chat.Net.Tests.Integration
{
    using System;
    using System.Threading.Tasks;

    using FluentAssertions;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using Ploeh.AutoFixture;
    using Rocket.Chat.Net.Models.LoginOptions;
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
            var loginResult = await RocketChatDriver.LoginWithEmailAsync(new EmailLoginOption() { Email = Constants.OneEmail, Password = Constants.OnePassword });

            // Assert
            loginResult.Should().NotBeNull();
            loginResult.HasError.Should().BeFalse();
            loginResult.Result.Token.Should().NotBeNull();
        }

        [Fact]
        public async Task Can_login_with_username()
        {
            // Act
            var loginResult = await RocketChatDriver.LoginWithUsernameAsync(new UsernameLoginOption() { Username = Constants.OneUsername, Password = Constants.OnePassword });

            // Assert
            loginResult.Should().NotBeNull();
            loginResult.HasError.Should().BeFalse();
            loginResult.Result.Token.Should().NotBeNull();
        }

        [Fact]
        public async Task Can_login_with_token()
        {
            var tokenResult = await RocketChatDriver.LoginWithUsernameAsync(new UsernameLoginOption() { Username = Constants.OneUsername, Password = Constants.OnePassword });

            // Act
            var loginResult = await RocketChatDriver.LoginResumeAsync(new ResumeLoginOption() { Token = tokenResult.Result.Token });

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
                await RocketChatDriver.LoginWithUsernameAsync(new UsernameLoginOption() { Username = Constants.OneUsername, Password = AutoFixture.Create<string>() });

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
                    RocketChatDriver.LoginWithUsernameAsync(new UsernameLoginOption() { Username = Constants.OneUsername, Password = AutoFixture.Create<string>() });

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

        [Fact]
        public void Seralization_of_username_login()
        {
            // Act 
            var options = AutoFixture.Build<UsernameLoginOption>().Without(p => p.TOTPSeed).Without(p => p.TOTPToken).Create();

            var jO = JObject.FromObject(options);

            // Assert
            Assert.True(jO.ContainsKey("user"));
            Assert.Equal(options.Username, jO["user"]);
        }

    }
}