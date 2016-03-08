namespace Rocket.Chat.Net.Models.Results
{
    public class LoginResult : BaseResult
    {
        public object Id { get; set; }

        public object Token { get; set; }

        public object TokenExpires { get; set; }
    }
}