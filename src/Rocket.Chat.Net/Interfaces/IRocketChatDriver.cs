namespace Rocket.Chat.Net.Interfaces
{
    using System;
    using System.Threading.Tasks;

    using Rocket.Chat.Net.Models;

    public interface IRocketChatDriver : IDisposable
    {
        event MessageReceived MessageReceived;
        Task ConnectAsync();
        Task SubscribeToRoomAsync(string roomId);
        Task<dynamic> LoginWithEmailAsync(string email, string password);
        Task LoginWithLdapAsync(string username, string password);
        Task<dynamic> GetRoomIdAsync(string roomId);
        Task JoinRoomAsync(string roomId);
        Task<dynamic> SendMessageAsync(string text, string roomId);
        Task<dynamic> LoginWithUsernameAsync(string username, string password);
    }
}