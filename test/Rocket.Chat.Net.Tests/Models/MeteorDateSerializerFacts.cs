namespace Rocket.Chat.Net.Tests.Models
{
    using System;

    using FluentAssertions;

    using Newtonsoft.Json;

    using Ploeh.AutoFixture;

    using Rocket.Chat.Net.Tests.Helpers;

    using Xunit;

    [Trait("Category", "Models")]
    public class MeteorDateSerializerFacts
    {
        private readonly Fixture _autoFixture = new Fixture();

        [Fact]
        public void Can_write()
        {
            var fixture = _autoFixture.Create<MeteorDateSerializerModel>();

            // Act
            var json = JsonConvert.SerializeObject(fixture);

            // Assert
            var epoch = ToEpoch(fixture.DateTime);
            var result = $"{{\"{nameof(fixture.DateTime)}\":{{\"$date\":{epoch}}}}}";
            json.Should().Be(result);
        }

        [Fact]
        public void When_null_write_null()
        {
            var fixture = new MeteorDateSerializerModel
            {
                DateTime = null
            };

            // Act
            var json = JsonConvert.SerializeObject(fixture);

            // Assert
            var result = $"{{\"{nameof(fixture.DateTime)}\":null}}";
            json.Should().Be(result);
        }

        [Fact]
        public void Can_read()
        {
            var fixture = _autoFixture.Create<MeteorDateSerializerModel>();
            var epoch = ToEpoch(fixture.DateTime);
            var result = $"{{\"{nameof(fixture.DateTime)}\":{{\"$date\":{epoch}}}}}";

            // Act
            var obj = JsonConvert.DeserializeObject<MeteorDateSerializerModel>(result);

            // Assert
            // HACK https://github.com/dennisdoomen/FluentAssertions/issues/359
            obj.DateTime?.ToLongDateString()
               .Should()
               .Be(fixture.DateTime?.ToUniversalTime().ToLongDateString());
        }

        [Fact]
        public void When_null_read_null()
        {
            MeteorDateSerializerModel fixture;
            var result = $"{{\"{nameof(fixture.DateTime)}\":null}}";

            // Act
            var obj = JsonConvert.DeserializeObject<MeteorDateSerializerModel>(result);

            // Assert
            obj.DateTime.Should().NotHaveValue();
        }

        [Fact]
        public void When_not_existing_read_null()
        {
            const string result = "{}";

            // Act
            var obj = JsonConvert.DeserializeObject<MeteorDateSerializerModel>(result);

            // Assert
            obj.DateTime.Should().NotHaveValue();
        }

        private static long ToEpoch(DateTime? date)
        {
            if (date == null)
            {
                return -1;
            }
            var span = date.Value.ToUniversalTime() - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return Convert.ToInt64(span.TotalMilliseconds);
        }
    }
}