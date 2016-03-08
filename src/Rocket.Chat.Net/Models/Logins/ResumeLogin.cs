namespace Rocket.Chat.Net.Models.Logins
{
    using Rocket.Chat.Net.Interfaces;

    public class ResumeLogin : ILogin
    {
        public string Token { get; set; }
    }
}