namespace Rocket.Chat.Net.Portability.Crypto
{
    using System;

    using Rocket.Chat.Net.Portability.Contracts;

    public class ShaHelper : ShaHelperBase
    {
        public override string Sha256Hash(string value)
        {
            throw new NotImplementedException();
        }
    }
}