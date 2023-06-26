namespace Rocket.Chat.Net.Tests.Integration
{
    using System.Threading.Tasks;

    using FluentAssertions;

    using Ploeh.AutoFixture;
    using Rocket.Chat.Net.Models.LoginOptions;
    using Rocket.Chat.Net.Tests.Helpers;

    using Xunit;
    using Xunit.Abstractions;

    [Trait("Category", "Driver")]
    public class ClientFacts : DriverFactsBase
    {
        public ClientFacts(ITestOutputHelper helper) : base(helper)
        {
        }

        [Fact]
        public async Task Ping_should_send_keep_alive()
        {
            await DefaultAccountLoginAsync();

            // Act
            await RocketChatDriver.PingAsync();

            // Assert
        }

        [Fact]
        public async Task FullUserData_returns_user_data()
        {
            await DefaultAccountLoginAsync();

            // Act
            var userData = await RocketChatDriver.GetFullUserDataAsync(Constants.TwoUsername);

            // Assert
            userData.Username.Should().Be(Constants.TwoUsername);
            userData.Emails.Should().Contain(x => x.Address.Contains(Constants.TwoEmail));
        }

        [Fact]
        public async Task FullUserData_returns_null_when_doesnt_exist()
        {
            var username = AutoFixture.Create<string>();
            await DefaultAccountLoginAsync();

            // Act
            var userData = await RocketChatDriver.GetFullUserDataAsync(username);

            // Assert
            userData.Should().BeNull();
        }


        [Fact]
        public async Task Rest_login()
        {
            var option = new UsernameLoginOption() { Username = Constants.OneUsername, Password = Constants.OnePassword };
            await RocketChatDriver.LoginRestApi(option);
        }
    }
}