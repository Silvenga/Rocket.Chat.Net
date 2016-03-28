namespace Rocket.Chat.Net.Tests.Integration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using FluentAssertions;

    using Ploeh.AutoFixture;

    using Rocket.Chat.Net.Driver;
    using Rocket.Chat.Net.Models;
    using Rocket.Chat.Net.Tests.Helpers;

    using Xunit;
    using Xunit.Abstractions;

    [Trait("Category", "Driver")]
    [Collection("Driver")]
    public class MessagingFacts : DriverFactsBase
    {
        // List of rooms to clean up - rain or shine.
        private IList<string> RoomsCreatedByName { get; } = new List<string>();
        private readonly AutoResetEvent _messageReceived = new AutoResetEvent(false);

        public MessagingFacts(ITestOutputHelper helper) : base(helper)
        {
        }

        [Fact]
        public async Task Can_send_messages()
        {
            await DefaultAccountLogin();

            var text = AutoFixture.Create<string>();
            var roomName = AutoFixture.Create<string>();
            var roomId = await CreateRoom(roomName);
            await RocketChatDriver.SubscribeToRoomAsync(roomId);

            RocketMessage message = null;
            RocketChatDriver.MessageReceived += rocketMessage =>
            {
                if (rocketMessage.RoomId == roomId)
                {
                    message = rocketMessage;
                    _messageReceived.Set();
                }
            };

            // Act
            await RocketChatDriver.SendMessageAsync(text, roomId);

            _messageReceived.WaitOne(TimeSpan.FromSeconds(5));

            // Assert
            message.Should().NotBeNull();
        }

        [Fact]
        public async Task When_bot_is_mentioned_set_flag()
        {
            await DefaultAccountLogin();

            var text = AutoFixture.Create<string>() + " @" + Constants.Username;
            var roomName = AutoFixture.Create<string>();
            var roomId = await CreateRoom(roomName);
            await RocketChatDriver.SubscribeToRoomAsync(roomId);

            RocketMessage message = null;
            RocketChatDriver.MessageReceived += rocketMessage =>
            {
                if (rocketMessage.RoomId == roomId)
                {
                    message = rocketMessage;
                    _messageReceived.Set();
                }
            };

            // Act
            await RocketChatDriver.SendMessageAsync(text, roomId);

            _messageReceived.WaitOne(TimeSpan.FromSeconds(5));

            // Assert
            message.Should().NotBeNull();
            message.IsBotMentioned.Should().BeTrue();
        }

        private async Task<string> CreateRoom(string roomName)
        {
            RoomsCreatedByName.Add(roomName);

            var roomResult = await RocketChatDriver.CreateRoomAsync(roomName);

            return roomResult.Result.RoomId;
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