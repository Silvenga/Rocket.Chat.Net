﻿namespace Rocket.Chat.Net.Models.LoginOptions
{
    using OtpNet;
    using System.Diagnostics.CodeAnalysis;

    using Rocket.Chat.Net.Interfaces;
    using System.Text.Json.Serialization;

    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
    public class ResumeLoginOption : ILoginOption
    {
        private Totp totpSeed;
        private string totpToken;

        /// <summary>
        /// Active login token given from a successful, previous login.
        /// </summary>
        public string Token { get; set; }
        
        [JsonIgnore]
        public Totp TOTPSeed
        {
            get => totpSeed;
            set => totpSeed = value;
        }

        [JsonIgnore]
        public string TOTPToken
        {
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