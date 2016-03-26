namespace Rocket.Chat.Net.Models.LoginOptions
{
    using System.Diagnostics.CodeAnalysis;

    using Rocket.Chat.Net.Interfaces;

    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
    public class UsernameLoginOption : ILoginOption
    {
        /// <summary>
        /// Username of the user to login as. This should not be a email address. 
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Plaintext password of the user. 
        /// </summary>
        public string Password { get; set; }
    }
}