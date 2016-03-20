namespace Rocket.Chat.Net.Models.Logins
{
    using System.Diagnostics.CodeAnalysis;

    using Rocket.Chat.Net.Interfaces;

    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
    public class ResumeLoginOption : ILoginOption
    {
        /// <summary>
        /// Active login token given from a successful, previous login.
        /// </summary>
        public string Token { get; set; }
    }
}