namespace Rocket.Chat.Net.Tests.Integration
{
    using System.Threading.Tasks;

    using Ploeh.AutoFixture;

    using Rocket.Chat.Net.Tests.Helpers;

    using Xunit;
    using Xunit.Abstractions;

    [Trait("Category", "Driver")]
    [Collection("Driver")]
    public class MessagingFacts : DriverFactsBase
    {
        public MessagingFacts(ITestOutputHelper helper) : base(helper)
        {
        }

        [Fact]
        public async Task Can_send_messages()
        {
            var message = AutoFixture.Create<string>();
            var roomId = "GENEREL";

            // Act
            await RocketChatDriver.ConnectAsync();
            await RocketChatDriver.LoginWithEmailAsync(Constants.Email, Constants.Password);
            var response = await RocketChatDriver.SendMessageAsync(message, roomId);

            // Assert
        }
    }
}