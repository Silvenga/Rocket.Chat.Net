namespace Rocket.Chat.Net.Models.Logins
{
    using Rocket.Chat.Net.Interfaces;

    public class UsernameLogin : ILogin
    {
        public string Username { get; set; }

        public string Password { get; set; }
    }
}