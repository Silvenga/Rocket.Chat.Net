namespace Rocket.Chat.Net.Interfaces
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Newtonsoft.Json.Linq;

    using Rocket.Chat.Net.Models;

    public interface IDdpClient : IDisposable
    {
        string Url { get; }
        string SessionId { get; }
        bool IsDisposed { get; }

        event DataReceived DataReceivedRaw;
        event DdpReconnect DdpReconnect;

        Task PingAsync(CancellationToken token);
        Task ConnectAsync(CancellationToken token);
        Task<string> SubscribeAsync(string name, CancellationToken token, params object[] args);
        Task<string> SubscribeAndWaitAsync(string name, CancellationToken token, params object[] args);
        Task<JObject> CallAsync(string method, CancellationToken token, params object[] args);
        Task UnsubscribeAsync(string id, CancellationToken token);
    }
}