namespace Rocket.Chat.Net.Tests.Models
{
    using FluentAssertions;

    using Newtonsoft.Json;

    using Ploeh.AutoFixture;

    using Rocket.Chat.Net.Tests.Helpers;

    using Xunit;

    public class RocketReactionConverterFacts
    {
        private static readonly Fixture AutoFixture = new Fixture();

        [Fact]
        public void Can_read()
        {
            var expected = new
            {
                One = new
                {
                    Name = AutoFixture.Create<string>(),
                    User = AutoFixture.Create<string>(),
                },
                Two = new
                {
                    Name = AutoFixture.Create<string>(),
                    User = AutoFixture.Create<string>(),
                }
            };

            var json =
                $"{{\"reactions\":{{\"{expected.One.Name}\":{{\"usernames\":[\"{expected.One.User}\"]}}, \"{expected.Two.Name}\":{{\"usernames\":[\"{expected.Two.User}\"]}} }}}}";

            // Act
            var obj = JsonConvert.DeserializeObject<RocketReactionConverterModel>(json);

            // Assert
            obj.Reactions.Count.Should().Be(2);
            obj.Reactions[0].Name.Should().Be(expected.One.Name);
            obj.Reactions[0].Usernames[0].Should().Be(expected.One.User);

            obj.Reactions[1].Name.Should().Be(expected.Two.Name);
            obj.Reactions[1].Usernames[0].Should().Be(expected.Two.User);
        }

        [Fact]
        public void When_reactions_is_empty_return_empty_list()
        {
            const string json = "{reactions : {}}";

            // Act
            var obj = JsonConvert.DeserializeObject<RocketReactionConverterModel>(json);

            // Assert
            obj.Reactions.Should().NotBeNull();
            obj.Reactions.Should().HaveCount(0);
        }

        [Fact]
        public void When_reactions_is_null_return_empty_list()
        {
            const string json = "{reactions : null}";

            // Act
            var obj = JsonConvert.DeserializeObject<RocketReactionConverterModel>(json);

            // Assert
            obj.Reactions.Should().NotBeNull();
            obj.Reactions.Should().HaveCount(0);
        }

        [Fact]
        public void When_reactions_is_missing_return_null()
        {
            const string json = "{}";

            // Act
            var obj = JsonConvert.DeserializeObject<RocketReactionConverterModel>(json);

            // Assert
            obj.Reactions.Should().BeNull();
        }

        [Fact]
        public void Can_write()
        {
            var expected = AutoFixture.Create<RocketReactionConverterModel>();

            // Act
            var json = JsonConvert.SerializeObject(expected);
            var result = JsonConvert.DeserializeObject<RocketReactionConverterModel>(json);

            // Assert
            result.Reactions.ShouldAllBeEquivalentTo(expected.Reactions);
        }
    }
}