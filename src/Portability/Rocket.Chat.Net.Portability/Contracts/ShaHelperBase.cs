namespace Rocket.Chat.Net.Portability.Contracts
{
    public abstract class ShaHelperBase
    {
        public abstract string Sha256Hash(string value);
    }
}