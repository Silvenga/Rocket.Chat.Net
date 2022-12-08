namespace Rocket.Chat.Net.Tests.Integration
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Rocket.Chat.Net.Driver;
    using Rocket.Chat.Net.Tests.Helpers;

    using NLog;

    using Xunit;
    using Xunit.Abstractions;

    [Trait("Category", "Driver")]
    public class DdpFacts : IDisposable
    {
        private readonly ILogger _helper;

        private CancellationToken TimeoutToken => CreateTimeoutToken();

        public DdpFacts(ITestOutputHelper helper)
        {
            _helper = NLog.LogManager.GetCurrentClassLogger();
        }

        [Fact]
        public async Task Can_ping()
        {
            var client = new DdpClient(Constants.RocketServer, false, _helper);
            await client.ConnectAsync(TimeoutToken);

            // Act
            await client.PingAsync(TimeoutToken);

            // Assert
        }

        private CancellationToken CreateTimeoutToken()
        {
            const int timeoutSeconds = 30;
            var source = new CancellationTokenSource();
            source.CancelAfter(TimeSpan.FromSeconds(timeoutSeconds));

            return source.Token;
        }

        public void Dispose()
        {
        }
    }
}