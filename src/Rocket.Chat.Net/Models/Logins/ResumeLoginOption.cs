namespace Rocket.Chat.Net.Models.Logins
{
    using Rocket.Chat.Net.Interfaces;

    public class ResumeLoginOption : ILoginOption
    {
        public string Token { get; set; }
    }
}