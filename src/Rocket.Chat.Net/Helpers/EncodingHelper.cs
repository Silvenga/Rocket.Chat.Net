namespace Rocket.Chat.Net.Helpers
{
    using System;
    using System.IO;
    using System.Text;
    using System.Security.Cryptography;

    public static class EncodingHelper
    {
        public const string Sha256 = "sha-256";

        public static string Sha256Hash(string value)
        {
            var builder = new StringBuilder();
            var encoding = Encoding.UTF8;

            using (var hash = SHA256.Create())
            {
                var result = hash.ComputeHash(encoding.GetBytes(value));
                foreach (var b in result)
                {
                    builder.Append(b.ToString("x2"));
                }
            }

            return builder.ToString();
        }

        public static string ConvertToBase64(Stream stream)
        {
            using (var bufferStream = new MemoryStream())
            {
                stream.CopyTo(bufferStream);
                return Convert.ToBase64String(bufferStream.ToArray());
            }
        }
    }
}