namespace Rocket.Chat.Net.Tests.Integration
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using FluentAssertions;

    using Ploeh.AutoFixture;

    using Rocket.Chat.Net.Driver;
    using Rocket.Chat.Net.Interfaces;
    using Rocket.Chat.Net.Models;
    using Rocket.Chat.Net.Tests.Helpers;

    using Xunit;
    using Xunit.Abstractions;

    [Trait("Category", "Driver")]
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

            await DefaultAccountLoginAsync();

            // Act
            var result = await RocketChatDriver.GetRoomIdAsync(roomName);

            // Assert
            result.Result.Should().Be(roomId);
        }

        [Fact]
        public async Task Lookup_of_non_existing_room_returns_null()
        {
            // Used to return room id, now throws a 500 as room is null

            var roomName = AutoFixture.Create<string>();

            await DefaultAccountLoginAsync();

            // Act
            var result = await RocketChatDriver.GetRoomIdAsync(roomName);

            // Assert
            result.HasError.Should().BeTrue();
            result.Result.Should().BeNull();
            result.Error.Reason.Should().Be("Not allowed");
        }

        [Fact]
        public async Task Create_direct_message_that_does_not_exist_should_create_room()
        {
            var roomName = $"{Constants.TwoUsername}";
            RoomsCreatedByName.Add(roomName);

            await DefaultAccountLoginAsync();

            await RocketChatDriver.SubscribeToRoomListAsync();

            // Act
            var result = await RocketChatDriver.CreatePrivateMessageAsync(roomName);

            // Assert
            result.HasError.Should().BeFalse();
        }

        [Fact]
        public async Task Create_direct_message_that_does_exist_should_update_room()
        {
            var roomName = $"{Constants.TwoUsername}";
            RoomsCreatedByName.Add(roomName);

            await DefaultAccountLoginAsync();

            await RocketChatDriver.SubscribeToRoomListAsync();

            // Act
            var result = await RocketChatDriver.CreatePrivateMessageAsync(Constants.TwoUsername);
            var result2 = await RocketChatDriver.CreatePrivateMessageAsync(Constants.TwoUsername);

            // Assert
            result.HasError.Should().BeFalse();
            result2.HasError.Should().BeFalse();
        }

        [Fact]
        public async Task Create_room_that_does_not_exist_should_create_room()
        {
            var roomName = AutoFixture.Create<string>();
            RoomsCreatedByName.Add(roomName);

            await DefaultAccountLoginAsync();

            // Act
            var result = await RocketChatDriver.CreateChannelAsync(roomName);

            // Assert
            result.HasError.Should().BeFalse();
        }

        [Fact]
        public async Task Create_group_that_does_not_exist_should_create_group()
        {
            var roomName = AutoFixture.Create<string>();
            RoomsCreatedByName.Add(roomName);

            await DefaultAccountLoginAsync();

            // Act
            var result = await RocketChatDriver.CreateGroupAsync(roomName);

            // Assert
            result.HasError.Should().BeFalse();
        }

        [Fact]
        public async Task Create_room_that_does_exist_should_return_error()
        {
            var roomName = AutoFixture.Create<string>();
            RoomsCreatedByName.Add(roomName);

            await DefaultAccountLoginAsync();

            await RocketChatDriver.CreateChannelAsync(roomName);

            // Act
            var result = await RocketChatDriver.CreateChannelAsync(roomName);

            // Assert
            result.HasError.Should().BeTrue();
            result.Error.Error.Should().Be("error-duplicate-channel-name");
        }

        [Fact]
        public async Task Delete_room_that_exists_should_delete_room()
        {
            var roomName = AutoFixture.Create<string>();
            RoomsCreatedByName.Add(roomName);

            await DefaultAccountLoginAsync();
            var room = await RocketChatDriver.CreateChannelAsync(roomName);

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

            await DefaultAccountLoginAsync();

            // Act
            var result = await RocketChatDriver.EraseRoomAsync(roomId);

            // Assert
            result.HasError.Should().BeTrue();
            // Currently any issues returns 500, this will hopefully change later
            result.Error.Error.Should().Be("500");
        }

        [Fact]
        public async Task Room_subscription_should_contain_general_room()
        {
            await DefaultAccountLoginAsync();

            // Act
            await RocketChatDriver.SubscribeToRoomListAsync();

            // Assert
            var collection = RocketChatDriver.GetRooms().ToList();
            collection.Should().ContainSingle(x => x.RoomId == "GENERAL")
                      .Which.Type.Should().Be(RoomType.Channel);
        }

        [Fact]
        public async Task Room_subscription_should_contain_general_room_info()
        {
            await DefaultAccountLoginAsync();

            // Act
            await RocketChatDriver.SubscribeToRoomListAsync();

            var collection = RocketChatDriver.GetRoomInfoCollection();

            // Assert
            var room = collection.GetById("GENERAL");
            room.Should().NotBeNull();
        }

        [Fact]
        public async Task Room_should_contain_general_room_with_info()
        {
            await DefaultAccountLoginAsync();

            // Act
            await RocketChatDriver.SubscribeToRoomListAsync();

            var collection = RocketChatDriver.Rooms;

            // Assert
            var room = collection.FirstOrDefault(x => x.Id == "GENERAL");
            room.Should().NotBeNull();
        }

        [Fact]
        public async Task List_channel_should_return_list_of_channels()
        {
            await DefaultAccountLoginAsync();

            // Act
            var result = await RocketChatDriver.ChannelListAsync();

            // Assert
            result.HasError.Should().BeFalse();
            result.Result.Channels.Should().Contain(x => x.Name == "general" && x.Id == "GENERAL");
        }

        private readonly Task<IRocketChatDriver> _composeTask = ComposeAsync();

        private static async Task<IRocketChatDriver> ComposeAsync()
        {
            var driver = new RocketChatDriver(Constants.RocketServer, false);
            await driver.ConnectAsync();
            await driver.LoginWithEmailAsync(Constants.OneEmail, Constants.OnePassword);
            await driver.SubscribeToRoomListAsync();

            return driver;
        }

        public override void Dispose()
        {
            base.Dispose();

            if (!RoomsCreatedByName.Any())
            {
                return;
            }
            using (var driver = _composeTask.Result)
            {
                var rooms = driver.Rooms;
                var toDelete = rooms.Where(x => RoomsCreatedByName.Contains(x.Name));

                foreach (var room in toDelete)
                {
                    driver.EraseRoomAsync(room.Id).Wait();
                }
            }
        }
    }
}