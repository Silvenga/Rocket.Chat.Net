namespace Rocket.Chat.Net.Models.Logins
{
    using Rocket.Chat.Net.Interfaces;

    public class EmailLogin : ILogin
    {
        public string Email { get; set; }

        public string Password { get; set; }
    }
}