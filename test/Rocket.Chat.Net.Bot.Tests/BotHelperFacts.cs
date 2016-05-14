namespace Rocket.Chat.Net.Bot.Tests
{
    using FluentAssertions;

    using Ploeh.AutoFixture;

    using Rocket.Chat.Net.Bot.Helpers;
    using Rocket.Chat.Net.Models;

    using Xunit;

    public class BotHelperFacts
    {
        private static readonly Fixture AutoFixture = new Fixture();

        [Fact]
        public void Create_basic_message_creates_message()
        {
            var rocketMessage = AutoFixture.Create<RocketMessage>();
            var reply = AutoFixture.Create<string>();

            // Act
            var result = BotHelper.CreateBasicReply(rocketMessage, reply);

            // Assert
            result.Message.Should().Be(reply);
            result.RoomId.Should().Be(rocketMessage.RoomId);
        }
    }
}