namespace Rocket.Chat.Net.Interfaces
{
    using System;
    using System.Threading.Tasks;

    using Rocket.Chat.Net.Models;

    public interface IChatDriver : IDisposable
    {
        event MessageReceived MessageReceived;
        Task ConnectAsync();
        Task SubscribeToRoomAsync(string roomId);
        Task<object> LoginWithPasswordAsync(string userName, string password);
        Task LoginWithLdapAsync(string username, string password);
        Task<object> GetRoomIdAsync(string roomId);
        Task JoinRoomAsync(string roomId);
        Task<object> SendMessageAsync(string text, string roomId);
    }
}