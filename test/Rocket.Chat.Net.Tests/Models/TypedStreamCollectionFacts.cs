namespace Rocket.Chat.Net.Tests.Models
{
    using FluentAssertions;

    using Newtonsoft.Json.Linq;

    using Ploeh.AutoFixture;

    using Rocket.Chat.Net.Collections;
    using Rocket.Chat.Net.Interfaces;
    using Rocket.Chat.Net.Tests.Models.Fixtures;

    using Xunit;

    public class TypedStreamCollectionFacts
    {
        private static readonly Fixture AutoFixture = new Fixture();

        private readonly string _name = AutoFixture.Create<string>();

        [Fact]
        public void When_acessing_name_it_should_pull_from_underlining_collection()
        {
            IStreamCollection fixture = new StreamCollection(_name);
            var collection = new TypedStreamCollection<StreamCollectionFixture>(fixture);

            // Act
            var name = collection.Name;

            // Assert
            name.Should().Be(_name);
        }
    }
}