namespace Rocket.Chat.Net.Tests.Helpers
{
    using System;
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

        protected async Task DefaultAccountLogin()
        {
            await RocketChatDriver.ConnectAsync();
            var result = await RocketChatDriver.LoginWithEmailAsync(Constants.Email, Constants.Password);
            result.HasError.Should().BeFalse();
        }

        public void Dispose()
        {
            RocketChatDriver.Dispose();
            XUnitLogger.Dispose();
        }
    }
}