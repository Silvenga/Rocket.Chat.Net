namespace Rocket.Chat.Net.Interfaces.Driver
{
    using System.IO;
    using System.Threading.Tasks;

    using Newtonsoft.Json.Linq;

    using Rocket.Chat.Net.Models;
    using Rocket.Chat.Net.Models.MethodResults;

    public interface IRocketUserManagement
    {
        /// <summary>
        /// Subscribe to a user's data
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        Task<FullUser> GetFullUserDataAsync(string username);

        /// <summary>
        /// TODO
        /// </summary>
        /// <returns></returns>
        Task<MethodResult> RemoveOtherTokensAsync();

        /// <summary>
        /// TODO
        /// </summary>
        /// <returns></returns>
        Task<MethodResult<LoginResult>> GetNewTokenAsync();

        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="name"></param>
        /// <param name="emailOrUsername"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        Task<JObject> RegisterUserAsync(string name, string emailOrUsername, string password);

        /// <summary>
        /// Reset the user's avatar - "Use your username initials"
        /// </summary>
        /// <returns></returns>
        Task<MethodResult> ResetAvatarAsync();

        /// <summary>
        /// Set the user's avatar to a URL. 
        /// </summary>
        /// <param name="url">An accessable URL by the server. Must be valid.</param>
        /// <returns></returns>
        Task<MethodResult> SetAvatarFromUrlAsync(string url);

        /// <summary>
        /// Set the user's avatar to an uploaded image.
        /// </summary>
        /// <param name="sourceStream">The source of the image.</param>
        /// <param name="mimeType">The mime type of the uploaded image. (e.g. "image/png")</param>
        /// <returns></returns>
        Task<MethodResult> SetAvatarFromImageStreamAsync(Stream sourceStream, string mimeType);

        /// <summary>
        /// Subscribe to the filtered users list. Can be called multiple times with different filter information.
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        Task SubscribeToFilteredUsersAsync(string username = "");

        /// <summary>
        /// The currently logged in user's id
        /// </summary>
        string UserId { get; }

        /// <summary>
        /// The currently logged in user's name
        /// </summary>
        string Username { get; }

        /// <summary>
        /// If the bot flag should be sent with messages. 
        /// This is recommended if the driver is not being operated by a human. 
        /// </summary>
        bool IsBot { get; set; }
    }
}