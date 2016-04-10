namespace Rocket.Chat.Net.Tests.Bot
{
    using FluentAssertions;

    using Ploeh.AutoFixture;

    using Rocket.Chat.Net.Helpers;
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
            var result = rocketMessage.CreateBasicReply(reply);

            // Assert
            result.Message.Should().Be(reply);
            result.RoomId.Should().Be(rocketMessage.RoomId);
        }
    }
}