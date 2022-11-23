using OtpNet;

namespace Rocket.Chat.Net.Interfaces
{
    public interface ILoginOption
    {
        Totp TOTPSeed
        {
            get;
            set;
        }

        string TOTPToken
        {
            get;
            set;
        }
    }
}