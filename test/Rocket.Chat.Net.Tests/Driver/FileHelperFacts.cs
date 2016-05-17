namespace Rocket.Chat.Net.Tests.Driver
{
    using System.IO;
    using System.Text;

    using FluentAssertions;

    using Rocket.Chat.Net.Helpers;

    using Xunit;

    public class FileHelperFacts
    {
        [Fact]
        public void Can_base64_string()
        {
            const string original = "qwertyuiop[asdfghjklzxcvbnm[]-=";
            var originalStream = new MemoryStream(Encoding.UTF8.GetBytes(original));

            const string expectedResult = "cXdlcnR5dWlvcFthc2RmZ2hqa2x6eGN2Ym5tW10tPQ==";

            // Act
            var result = FileHelper.ConvertToBase64(originalStream);

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
            var result = FileHelper.ConvertToBase64(originalStream);

            // Assert
            result.Should().Be(expectedResult);
        }
    }
}