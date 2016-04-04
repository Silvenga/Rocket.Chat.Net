namespace Rocket.Chat.Net.Tests.Integration
{
    using System.Threading.Tasks;

    using FluentAssertions;

    using Rocket.Chat.Net.Tests.Helpers;

    using Xunit;
    using Xunit.Abstractions;

    [Trait("Category", "Driver")]
    [Collection("Driver")]
    public class ClientFacts : DriverFactsBase
    {
        // TODO Fix make these tests test

        public ClientFacts(ITestOutputHelper helper) : base(helper)
        {
        }

        [Fact]
        public async Task Load_message_history_loads_past_messages()
        {
            await DefaultAccountLoginAsync();

            var result = await RocketChatDriver.LoadMessagesAsync("GENERAL");
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
            var userData = await RocketChatDriver.GetFullUserDataAsync(Constants.TestUsername);

            // Assert
            userData.Username.Should().Be(Constants.TestUsername);
            userData.Emails.Should().Contain(x => x.Address.Contains(Constants.TestEmail));
        }

        [Fact]
        public async Task FullUserData_returns_null_when_doesnt_exist()
        {
            const string userTest = "test";
            await DefaultAccountLoginAsync();

            // Act
            var userData = await RocketChatDriver.GetFullUserDataAsync(userTest);

            // Assert
            userData.Should().BeNull();
        }
    }
}