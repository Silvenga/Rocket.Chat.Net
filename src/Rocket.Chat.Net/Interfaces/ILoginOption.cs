using OtpNet;
using System.Text.Json.Serialization;

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