namespace Rocket.Chat.Net.Models.LoginOptions
{
    using System.Diagnostics.CodeAnalysis;

    using Rocket.Chat.Net.Interfaces;

    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
    public class LdapLoginOption : ILoginOption
    {
        /// <summary>
        /// Username of the user to login as. Do not include the domain in which this user resides. 
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Plaintext password of the user. 
        /// </summary>
        public string Password { get; set; }
    }
}