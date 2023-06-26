namespace Rocket.Chat.Net.Models.LoginOptions
{
    using Newtonsoft.Json;
    using OtpNet;
    using Rocket.Chat.Net.Interfaces;
    using System.Runtime.CompilerServices;

    public class EmailLoginOption : IRestLoginOption
    {

        private Totp totpSeed;
        private string totpToken;

        /// <summary>
        /// Email of the user to login as. Should be in the format of user@example.com.
        /// </summary>
        [JsonProperty(PropertyName = "user")]
        public string Email { get; set; }

        /// <summary>
        /// Plaintext password of the user.
        /// </summary>
        [JsonProperty(PropertyName = "password")]
        public string Password { get; set; }

        [JsonIgnore]
        public Totp TOTPSeed { 
            get => totpSeed ;
            set => totpSeed = value;
        }

        [JsonIgnore]
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