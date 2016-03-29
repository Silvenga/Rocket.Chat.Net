namespace Rocket.Chat.Net.Tests.Helpers
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using FluentAssertions;

    using Ploeh.AutoFixture;

    using Rocket.Chat.Net.Driver;
    using Rocket.Chat.Net.Interfaces;

    using Xunit.Abstractions;

    public class DriverFactsBase : IDisposable
    {
        protected readonly Fixture AutoFixture = new Fixture();
        protected readonly IRocketChatDriver RocketChatDriver;
        protected readonly XUnitLogger XUnitLogger;

        protected DriverFactsBase(ITestOutputHelper helper)
        {
            XUnitLogger = new XUnitLogger(helper);
            RocketChatDriver = new RocketChatDriver(Constants.RocketServer, false, XUnitLogger);
        }

        protected async Task Defaultaccountloginasync()
        {
            await RocketChatDriver.ConnectAsync();
            var result = await RocketChatDriver.LoginWithEmailAsync(Constants.Email, Constants.Password);
            result.HasError.Should().BeFalse();
        }

        protected async Task CleanupRoomsAsync()
        {
            var rooms = await RocketChatDriver.ChannelListAsync();
            rooms.HasError.Should().BeFalse();
            foreach (var room in rooms.Result.Channels.Where(x => x.Id != "GENERAL"))
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