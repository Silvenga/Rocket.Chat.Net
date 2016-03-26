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
            await DefaultAccountLogin();

            var result = await RocketChatDriver.LoadMessagesAsync("GENERAL");
        }

        [Fact]
        public async Task Ping_should_send_keep_alive()
        {
            await DefaultAccountLogin();

            // Act
            await RocketChatDriver.PingAsync();

            // Assert
        }

        [Fact]
        public async Task FullUserData_returns_user_data()
        {
            const string userTest = "test";
            await DefaultAccountLogin();

            // Act
            var userData = await RocketChatDriver.GetFullUserDataAsync(userTest);

            // Assert
            userData.Username.Should().Be(userTest);
            userData.Emails.Should().Contain(x => x.Address.Contains("test@silvenga.com"));
        }
    }
}