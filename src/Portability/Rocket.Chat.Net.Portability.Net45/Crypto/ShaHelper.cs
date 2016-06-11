namespace Rocket.Chat.Net.Portability.Crypto
{
    using System.Security.Cryptography;
    using System.Text;

    using Rocket.Chat.Net.Portability.Contracts;

    public class ShaHelper : ShaHelperBase
    {
        public override string Sha256Hash(string value)
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
    }
}