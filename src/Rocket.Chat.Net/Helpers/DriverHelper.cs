namespace Rocket.Chat.Net.Helpers
{
    using System.Security.Cryptography;
    using System.Text;

    using Newtonsoft.Json.Linq;

    using Rocket.Chat.Net.Models;

    public static class DriverHelper
    {
        public const string Sha256 = "sha-256";

        public static bool HasProperty(dynamic data, string name)
        {
            return data?[name] != null;
        }

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

        public static RocketMessage ParseMessage(dynamic data)
        {
            var message = ((JObject) data).ToObject<RocketMessage>();
            return message;
        }

        public static bool HasError(dynamic data)
        {
            return DriverHelper.HasProperty(data, "error");
        }
    }
}