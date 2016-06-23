namespace Rocket.Chat.Net.Tests.Helpers
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using FluentAssertions;

    using Newtonsoft.Json;

    using Ploeh.AutoFixture;

    using Rocket.Chat.Net.Driver;
    using Rocket.Chat.Net.Interfaces;

    using Xunit.Abstractions;

    public class DriverFactsBase : IDisposable
    {
        protected static readonly Fixture AutoFixture = new Fixture();
        protected readonly IRocketChatDriver RocketChatDriver;
        protected readonly XUnitLogger XUnitLogger;

        protected DriverFactsBase(ITestOutputHelper helper)
        {
            XUnitLogger = new XUnitLogger(helper);
            RocketChatDriver = new RocketChatDriver(Constants.RocketServer, false, XUnitLogger, jsonSerializerSettings: new JsonSerializerSettings());
        }

        protected async Task DefaultAccountLoginAsync()
        {
            await RocketChatDriver.ConnectAsync();
            var result = await RocketChatDriver.LoginWithEmailAsync(Constants.OneEmail, Constants.OnePassword);
            result.HasError.Should().BeFalse();
        }

        protected async Task CleanupRoomsAsync()
        {
            await RocketChatDriver.SubscribeToRoomListAsync();
            var rooms = RocketChatDriver.Rooms.ToList();

            foreach (var room in rooms.Where(x => x.Id != "GENERAL"))
            {
                await RocketChatDriver.EraseRoomAsync(room.Id);
            }
        }

        public virtual void Dispose()
        {
            RocketChatDriver.Dispose();
            XUnitLogger.Dispose();
        }
    }
}