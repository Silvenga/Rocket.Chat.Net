namespace Rocket.Chat.Net.Tests.Driver
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using FluentAssertions;

    using Newtonsoft.Json.Linq;

    using NSubstitute;

    using Ploeh.AutoFixture;

    using Rocket.Chat.Net.Collections;
    using Rocket.Chat.Net.Driver;
    using Rocket.Chat.Net.Helpers;
    using Rocket.Chat.Net.Interfaces;
    using Rocket.Chat.Net.Models;
    using Rocket.Chat.Net.Models.MethodResults;
    using Rocket.Chat.Net.Tests.Helpers;

    using Xunit;

    public class RocketChatDriverFacts
    {
        private static readonly Fixture AutoFixture = new Fixture();

        private readonly IDdpClient _mockClient;
        private readonly IStreamCollectionDatabase _mockCollectionDatabase;
        private readonly IRocketChatDriver _driver;

        private static CancellationToken CancellationToken => Arg.Any<CancellationToken>();

        public RocketChatDriverFacts()
        {
            _mockClient = Substitute.For<IDdpClient>();
            _mockCollectionDatabase = Substitute.For<IStreamCollectionDatabase>();
            var mockLog = Substitute.For<ILogger>();
            _driver = new RocketChatDriver(mockLog, _mockClient, _mockCollectionDatabase);
        }

        [Fact]
        public async Task Connect_should_connect_the_client()
        {
            // Act
            await _driver.ConnectAsync();

            // Assert
            await _mockClient.Received().ConnectAsync(CancellationToken);
        }

        [Fact]
        public async Task When_subscribing_to_channel_sub_with_client()
        {
            // Act
            await _driver.SubscribeToRoomAsync();

            // Assert
            await _mockClient.Received().SubscribeAsync("stream-messages", CancellationToken, null, "10");
        }

        [Fact]
        public async Task When_subscribing_to_ome_channel_sub_with_client()
        {
            var room = AutoFixture.Create<string>();

            // Act
            await _driver.SubscribeToRoomAsync(room);

            // Assert
            await _mockClient.Received().SubscribeAsync("stream-messages", CancellationToken, room, "10");
        }

        [Fact]
        public async Task Ping_server_uses_client_ping()
        {
            // Act
            await _driver.PingAsync();

            // Assert
            await _mockClient.Received().PingAsync(CancellationToken);
        }

        [Fact]
        public async Task Login_with_email()
        {
            var email = AutoFixture.Create<string>();
            var password = AutoFixture.Create<string>();
            var payload = new
            {
                user = new
                {
                    email
                },
                password = new
                {
                    digest = EncodingHelper.Sha256Hash(password),
                    algorithm = EncodingHelper.Sha256
                }
            };

            var loginResult = AutoFixture.Create<LoginResult>();
            var loginResponse = JObject.FromObject(new
            {
                result = loginResult
            });

            _mockClient.CallAsync(Arg.Any<string>(), CancellationToken, Arg.Any<object[]>())
                       .ReturnsForAnyArgs(Task.FromResult(loginResponse));

            IStreamCollection collection = new StreamCollection("users");
            var user = JObject.FromObject(new {username = ""});
            collection.Added(loginResult.UserId, user);
            _mockCollectionDatabase.WaitForObjectInCollectionAsync("users", loginResult.UserId, CancellationToken)
                                   .Returns(Task.FromResult(collection));

            // Act
            await _driver.LoginWithEmailAsync(new Net.Models.LoginOptions.EmailLoginOption { Email = email, Password = password });

            // Assert
            await _mockClient.ReceivedWithAnyArgs().CallAsync("login", CancellationToken, payload);
        }

        [Fact]
        public void When_login_option_is_unknown_throw()
        {
            var options = AutoFixture.Create<DummyLoginOption>();

            // Act
            Action action = () => _driver.LoginAsync(options).Wait();

            // 
            action.ShouldThrow<NotSupportedException>();
        }

        [Fact]
        public void Reconnect_should_bubble_up()
        {
            var called = false;
            _driver.DdpReconnect += () => called = true;

            // Act
            _mockClient.DdpReconnect += Raise.Event<DdpReconnect>();

            // Assert
            called.Should().BeTrue();
        }

        [Fact]
        public void Disposing_driver_should_dispose_client()
        {
            // Act
            _driver.Dispose();

            // Assert
            _mockClient.Received().Dispose();
        }
    }
}