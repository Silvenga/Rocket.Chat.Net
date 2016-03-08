namespace Rocket.Chat.Net.Models.Results
{
    using System;

    public class LoginResult : BaseResult
    {
        public string Id { get; set; }

        public string Token { get; set; }

        public DateTime TokenExpires { get; set; }
    }
}