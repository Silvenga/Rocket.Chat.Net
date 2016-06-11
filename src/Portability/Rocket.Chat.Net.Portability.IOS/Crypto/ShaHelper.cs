namespace Rocket.Chat.Net.Portability.Crypto
{
    using System.Text;

    using PCLCrypto;

    using Rocket.Chat.Net.Portability.Contracts;

    public class ShaHelper : ShaHelperBase
    {
        public override string Sha256Hash(string value)
        {
            var builder = new StringBuilder();
            var encoding = Encoding.UTF8;

            var hasher = WinRTCrypto.HashAlgorithmProvider.OpenAlgorithm(HashAlgorithm.Sha256);

            var result = hasher.HashData(encoding.GetBytes(value));
            foreach (var b in result)
            {
                builder.Append(b.ToString("x2"));
            }

            return builder.ToString();
        }
    }
}