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
            RocketChatDriver.ConnectAsync().Wait();
        }

        [Fact]
        public async Task Load_message_history_loads_past_messages()
        {
            var loginResult = await RocketChatDriver.LoginWithEmailAsync(Constants.Email, Constants.Password);

            var result = await RocketChatDriver.LoadMessagesAsync("GENERAL");
        }

        [Fact]
        public async Task List_channel_should_return_list_of_channels()
        {
            var loginResult = await RocketChatDriver.LoginWithEmailAsync(Constants.Email, Constants.Password);

            var result = await RocketChatDriver.ChannelListAsync();
        }

        [Fact]
        public async Task FullUserData_returns_user_data()
        {
            const string userTest = "test";
            await RocketChatDriver.LoginWithEmailAsync(Constants.Email, Constants.Password);

            // Act
            var userData = await RocketChatDriver.GetFullUserDataAsync(userTest);

            // Assert
            userData.Username.Should().Be(userTest);
            userData.Emails.Should().Contain(x => x.Address.Contains("test@silvenga.com"));
        }
    }
}