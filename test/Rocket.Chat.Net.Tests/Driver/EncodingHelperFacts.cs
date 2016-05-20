namespace Rocket.Chat.Net.Tests.Driver
{
    using System.IO;
    using System.Text;

    using FluentAssertions;

    using Rocket.Chat.Net.Helpers;

    using Xunit;

    public class EncodingHelperFacts
    {
        [Fact]
        public void Sha256_hash_is_correct()
        {
            const string original = "qwertyuiop[asdfghjklzxcvbnm[]-=";
            const string shaHash = "f0df4d7c843854c6c77401a086f76bfa5457ccffb2da59eb7e435e1d903a237a";

            // Act
            var result = EncodingHelper.Sha256Hash(original);

            // Assert
            result.Should().Be(shaHash);
        }

        [Fact]
        public void Can_base64_string()
        {
            const string original = "qwertyuiop[asdfghjklzxcvbnm[]-=";
            var originalStream = new MemoryStream(Encoding.UTF8.GetBytes(original));

            const string expectedResult = "cXdlcnR5dWlvcFthc2RmZ2hqa2x6eGN2Ym5tW10tPQ==";

            // Act
            var result = EncodingHelper.ConvertToBase64(originalStream);

            // Assert
            result.Should().Be(expectedResult);
        }

        [Fact]
        public void Can_base64_image()
        {
            const string originalFile = "Assets/random-image.png";
            var originalStream = File.OpenRead(originalFile);

            const string expectedResultFile = "Assets/random-image.png.base64";
            var expectedResult = File.ReadAllText(expectedResultFile);

            // Act
            var result = EncodingHelper.ConvertToBase64(originalStream);

            // Assert
            result.Should().Be(expectedResult);
        }
    }
}