namespace Rocket.Chat.Net.Models
{
    public class User
    {
        /// <summary>
        /// User Id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Username
        /// </summary>
        public string Username { get; set; }

        public override string ToString()
        {
            return $"{Username} ({Id})";
        }
    }
}