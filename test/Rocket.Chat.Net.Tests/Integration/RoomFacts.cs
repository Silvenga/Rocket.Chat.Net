namespace Rocket.Chat.Net.Tests.Integration
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using FluentAssertions;

    using Ploeh.AutoFixture;

    using Rocket.Chat.Net.Driver;
    using Rocket.Chat.Net.Tests.Helpers;

    using Xunit;
    using Xunit.Abstractions;

    public class RoomFacts : DriverFactsBase
    {
        public RoomFacts(ITestOutputHelper helper) : base(helper)
        {
        }

        // List of rooms to clean up - rain or shine.
        private IList<string> RoomsCreatedByName { get; } = new List<string>();

        [Fact]
        public async Task Lookup_of_valid_room_returns_roomId()
        {
            // GENERAL -> GENERAL
            const string roomName = "GENERAL";
            const string roomId = "GENERAL";

            await Defaultaccountloginasync();

            // Act
            var result = await RocketChatDriver.GetRoomIdAsync(roomName);

            // Assert
            result.Should().Be(roomId);
        }

        [Fact]
        public async Task Lookup_of_non_existing_room_returns_itself()
        {
            var roomName = AutoFixture.Create<string>();

            await Defaultaccountloginasync();

            // Act
            var result = await RocketChatDriver.GetRoomIdAsync(roomName);

            // Assert
            result.Should().Be(roomName);
        }

        [Fact]
        public async Task Create_room_that_does_not_exist_should_create_room()
        {
            var roomName = AutoFixture.Create<string>();
            RoomsCreatedByName.Add(roomName);

            await Defaultaccountloginasync();

            // Act
            var result = await RocketChatDriver.CreateRoomAsync(roomName);

            // Assert
            result.HasError.Should().BeFalse();
        }

        [Fact]
        public async Task Create_room_that_does_exist_should_return_error()
        {
            var roomName = AutoFixture.Create<string>();
            RoomsCreatedByName.Add(roomName);

            await Defaultaccountloginasync();

            await RocketChatDriver.CreateRoomAsync(roomName);

            // Act
            var result = await RocketChatDriver.CreateRoomAsync(roomName);

            // Assert
            result.HasError.Should().BeTrue();
            result.Error.Error.Should().Be("duplicate-name");
        }

        [Fact]
        public async Task Delete_room_that_exists_should_delete_room()
        {
            var roomName = AutoFixture.Create<string>();
            RoomsCreatedByName.Add(roomName);

            await Defaultaccountloginasync();
            var room = await RocketChatDriver.CreateRoomAsync(roomName);

            // Act
            var result = await RocketChatDriver.EraseRoomAsync(room.Result.RoomId);

            // Assert
            result.HasError.Should().BeFalse();
            result.Result.Should().Be(1);

            var rooms = await RocketChatDriver.ChannelListAsync();
            rooms.Result.Channels.Should().NotContain(x => x.Id == room.Id);
        }

        [Fact]
        public async Task Delete_room_that_does_not_exists_should_return_error()
        {
            var roomId = AutoFixture.Create<string>();

            await Defaultaccountloginasync();

            // Act
            var result = await RocketChatDriver.EraseRoomAsync(roomId);

            // Assert
            result.HasError.Should().BeTrue();
            // Currently any issues returns 500, this will hopefully change later
            result.Error.Error.Should().Be("500");
        }

        [Fact]
        public async Task List_channel_should_return_list_of_channels()
        {
            await Defaultaccountloginasync();

            // Act
            var result = await RocketChatDriver.ChannelListAsync();

            // Assert
            result.HasError.Should().BeFalse();
            result.Result.Channels.Should().Contain(x => x.Name == "general" && x.Id == "GENERAL");
        }

        public override void Dispose()
        {
            base.Dispose();

            if (!RoomsCreatedByName.Any())
            {
                return;
            }
            using (var driver = new RocketChatDriver(Constants.RocketServer, false))
            {
                driver.ConnectAsync().Wait();
                driver.LoginWithEmailAsync(Constants.Email, Constants.Password).Wait();
                var rooms = driver.ChannelListAsync().Result;
                var toDelete = rooms.Result.Channels.Where(x => RoomsCreatedByName.Contains(x.Name));

                foreach (var room in toDelete)
                {
                    driver.EraseRoomAsync(room.Id).Wait();
                }
            }
        }
    }
}