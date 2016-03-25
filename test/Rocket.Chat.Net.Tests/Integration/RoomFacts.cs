namespace Rocket.Chat.Net.Tests.Integration
{
    using System.Threading.Tasks;

    using FluentAssertions;

    using Ploeh.AutoFixture;

    using Rocket.Chat.Net.Tests.Helpers;

    using Xunit;
    using Xunit.Abstractions;

    public class RoomFacts : DriverFactsBase
    {
        public RoomFacts(ITestOutputHelper helper) : base(helper)
        {
            RocketChatDriver.ConnectAsync().Wait();
            RocketChatDriver.LoginWithEmailAsync(Constants.Email, Constants.Password).Wait();
        }

        [Fact]
        public async Task Lookup_of_valid_room_returns_roomId()
        {
            // GENERAL -> GENERAL
            const string roomName = "GENERAL";
            const string roomId = "GENERAL";

            // Act
            var result = await RocketChatDriver.GetRoomIdAsync(roomName);

            // Assert
            result.Should().Be(roomId);
        }

        [Fact]
        public async Task Lookup_of_non_existing_room_returns_itself()
        {
            var roomName = AutoFixture.Create<string>();

            // Act
            var result = await RocketChatDriver.GetRoomIdAsync(roomName);

            // Assert
            result.Should().Be(roomName);
        }

        [Fact]
        public async Task Create_room()
        {
            var roomName = AutoFixture.Create<string>();

            // Act

            // Assert
        }
    }
}