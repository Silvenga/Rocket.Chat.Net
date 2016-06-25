namespace Rocket.Chat.Net.Helpers
{
    using System;
    using System.IO;

    using Rocket.Chat.Net.Portability.Crypto;

    public static class EncodingHelper
    {
        public const string Sha256 = "sha-256";

        public static string Sha256Hash(string value)
        {
            var portableCrypto = new ShaHelper();
            return portableCrypto.Sha256Hash(value);
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