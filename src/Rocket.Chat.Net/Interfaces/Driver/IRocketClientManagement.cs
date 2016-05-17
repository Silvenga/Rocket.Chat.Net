namespace Rocket.Chat.Net.Interfaces.Driver
{
    using System.Threading.Tasks;

    using Rocket.Chat.Net.Models;
    using Rocket.Chat.Net.Models.MethodResults;

    public interface IRocketClientManagement
    {
        /// <summary>
        /// Connect via the DDP protocol using WebSockets
        /// </summary>
        /// <returns></returns>
        Task ConnectAsync();

        /// <summary>
        /// Login with email
        /// </summary>
        /// <param name="email">Email to use</param>
        /// <param name="password">Plaintext password to use (will be SHA-256 before sending)</param>
        /// <returns></returns>
        Task<MethodResult<LoginResult>> LoginWithEmailAsync(string email, string password);

        /// <summary>
        /// Login with LDAP
        /// </summary>
        /// <param name="username">Email/Username to use</param>
        /// <param name="password">Plaintext password to use</param>
        /// <returns></returns>
        Task<MethodResult<LoginResult>> LoginWithLdapAsync(string username, string password);

        /// <summary>
        /// Login with username
        /// </summary>
        /// <param name="username">Username to use</param>
        /// <param name="password">Plaintext password to use (will be SHA-256 before sending)</param>
        /// <returns></returns>
        Task<MethodResult<LoginResult>> LoginWithUsernameAsync(string username, string password);

        /// <summary>
        /// Resume a login session
        /// </summary>
        /// <param name="sessionToken">Active token given from a previous login</param>
        /// <returns></returns>
        Task<MethodResult<LoginResult>> LoginResumeAsync(string sessionToken);

        /// <summary>
        /// Login with a ILogin object
        /// </summary>
        /// <param name="loginOption">Login option to use</param>
        /// <returns></returns>
        Task<MethodResult<LoginResult>> LoginAsync(ILoginOption loginOption);

        /// <summary>
        /// Called when the Ddp Clients reconnects, can be used to log back in or resubscribe.
        /// </summary>
        event DdpReconnect DdpReconnect;

        /// <summary>
        /// Ping Rocket.Chat server (a kind of keep alive). 
        /// Can be used at any time when connected. 
        /// </summary>
        /// <returns></returns>
        Task PingAsync();

        /// <summary>
        /// Get a streaming collection.
        /// </summary>
        /// <param name="collectionName">Name of the collection to get (e.g. users).</param>
        /// <returns>Collection requested, null if it does not exist.</returns>
        IStreamCollection GetCollection(string collectionName);

        /// <summary>
        /// Subscribe to stream using no params
        /// </summary>
        /// <param name="streamName">Name of the stream to subscribe to</param>
        /// <param name="o">Parameters to include in the subscription</param>
        /// <returns></returns>
        Task SubscribeToAsync(string streamName, params object[] o);
    }
}