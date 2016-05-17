namespace Rocket.Chat.Net.Models.LoginOptions
{
    using Rocket.Chat.Net.Interfaces;
    
    public class EmailLoginOption : ILoginOption
    {
        /// <summary>
        /// Email of the user to login as. Should be in the format of user@example.com.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Plaintext password of the user.
        /// </summary>
        public string Password { get; set; }
    }
}