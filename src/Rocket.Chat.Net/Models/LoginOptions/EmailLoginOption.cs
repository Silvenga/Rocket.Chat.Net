namespace Rocket.Chat.Net.Models.LoginOptions
{
    using OtpNet;
    using Rocket.Chat.Net.Interfaces;
    using System.Runtime.CompilerServices;

    public class EmailLoginOption : ILoginOption
    {

        private Totp totpSeed;
        private string totpToken;

        /// <summary>
        /// Email of the user to login as. Should be in the format of user@example.com.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Plaintext password of the user.
        /// </summary>
        public string Password { get; set; }
        public Totp TOTPSeed { 
            get => totpSeed ;
            set => totpSeed = value;
        }

        public string TOTPToken { 
            get 
            {
                if (this.totpSeed != null)
                    totpToken = totpSeed.ComputeTotp();
                return totpToken;
            }
            set 
            {
                totpToken = value;
            }
        }
    }
}