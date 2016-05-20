namespace Rocket.Chat.Net.Helpers
{
    using System.IO;
    using System.Security.Cryptography;
    using System.Text;

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
            // TODO Check performace

            using (var workspace = new MemoryStream())
            using (var transform = new ToBase64Transform())
            using (var destination = new CryptoStream(workspace, transform, CryptoStreamMode.Write))
            using (var reader = new StreamReader(workspace))
            {
                stream.CopyTo(destination);
                destination.FlushFinalBlock();

                workspace.Position = 0;
                return reader.ReadToEnd();
            }
        }
    }
}