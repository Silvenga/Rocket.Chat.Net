namespace Rocket.Chat.Net.Tests.Driver
{
    using FluentAssertions;

    using Newtonsoft.Json.Linq;
    using NLog;
    using NSubstitute;

    using Ploeh.AutoFixture;

    using Rocket.Chat.Net.Driver;
    using Rocket.Chat.Net.Interfaces;
    using Rocket.Chat.Net.Models;

    using Xunit;

    public class MessagingFacts
    {
        private static readonly Fixture AutoFixture = new Fixture();

        private readonly IDdpClient _mockClient;
        private readonly IStreamCollectionDatabase _mockCollectionDatabase;
        private readonly IRocketChatDriver _driver;

        public MessagingFacts()
        {
            _mockClient = Substitute.For<IDdpClient>();
            _mockCollectionDatabase = Substitute.For<IStreamCollectionDatabase>();
            var mockLog = Substitute.For<ILogger>();
            _driver = new RocketChatDriver(mockLog, _mockClient, _mockCollectionDatabase);
        }

        [Fact]
        public void Added_message_should_add_to_a_streaming_collection()
        {
            var payload = new
            {
                collection = AutoFixture.Create<string>(),
                id = AutoFixture.Create<string>(),
                fields = new
                {
                    id = AutoFixture.Create<string>()
                }
            };

            var mockCollection = Substitute.For<IStreamCollection>();
            _mockCollectionDatabase.GetOrAddCollection(payload.collection).Returns(mockCollection);

            // Act
            _mockClient.DataReceivedRaw += Raise.Event<DataReceived>("added", JObject.FromObject(payload));

            // Assert
            mockCollection.Received().Added(payload.id, Arg.Any<JObject>());
        }

        [Fact]
        public void Changed_message_should_change_a_streaming_collection()
        {
            var payload = new
            {
                collection = AutoFixture.Create<string>(),
                id = AutoFixture.Create<string>(),
                fields = new
                {
                    id = AutoFixture.Create<string>()
                }
            };

            var mockCollection = Substitute.For<IStreamCollection>();
            _mockCollectionDatabase.GetOrAddCollection(payload.collection).Returns(mockCollection);

            // Act
            _mockClient.DataReceivedRaw += Raise.Event<DataReceived>("changed", JObject.FromObject(payload));

            // Assert
            mockCollection.Received().Changed(payload.id, Arg.Any<JObject>());
        }

        [Fact]
        public void Removed_message_should_change_a_streaming_collection()
        {
            var payload = new
            {
                collection = AutoFixture.Create<string>(),
                id = AutoFixture.Create<string>()
            };

            var mockCollection = Substitute.For<IStreamCollection>();
            _mockCollectionDatabase.GetOrAddCollection(payload.collection).Returns(mockCollection);

            // Act
            _mockClient.DataReceivedRaw += Raise.Event<DataReceived>("removed", JObject.FromObject(payload));

            // Assert
            mockCollection.Received().Removed(payload.id);
        }

        [Fact]
        public void Non_streaming_messages_should_not_change_collections()
        {
            var payload = new
            {
                id = AutoFixture.Create<string>(),
                msg = AutoFixture.Create<string>(),
                random = AutoFixture.Create<int>()
            };

            // Act
            _mockClient.DataReceivedRaw += Raise.Event<DataReceived>(payload.msg, JObject.FromObject(payload));

            // Assert
            _mockCollectionDatabase.DidNotReceive().GetOrAddCollection(Arg.Any<string>());
        }

        [Fact]
        public void Rocket_messages_should_be_forwarded()
        {
            var rocketMessage = AutoFixture.Create<RocketMessage>();
            var payload = new
            {
                id = AutoFixture.Create<string>(),
                msg = "changed",
                collection = "stream-room-messages",
                fields = new
                {
                    args = new object[]
                    {
                        rocketMessage
                    }
                }
            };

            RocketMessage result = null;
            _driver.MessageReceived += message => result = message;

            // Act
            _mockClient.DataReceivedRaw += Raise.Event<DataReceived>(payload.msg, JObject.FromObject(payload));

            // Assert
            result.Should().NotBeNull();
            result.Message.Should().Be(rocketMessage.Message);
        }

        [Fact]
        public void When_bot_is_mentioned_set_flag()
        {
            var rocketMessage = AutoFixture.Build<RocketMessage>()
                                           .Create();
            var payload = new
            {
                id = AutoFixture.Create<string>(),
                msg = "changed",
                collection = "stream-room-messages",
                fields = new
                {
                    args = new object[]
                    {
                        rocketMessage
                    }
                }
            };

            RocketMessage result = null;
            _driver.MessageReceived += message => result = message;

            // Act
            _mockClient.DataReceivedRaw += Raise.Event<DataReceived>(payload.msg, JObject.FromObject(payload));

            // Assert
            result.Should().NotBeNull();
            result.Message.Should().Be(rocketMessage.Message);
        }
    }
}