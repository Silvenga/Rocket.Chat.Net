namespace Rocket.Chat.Net.Tests.Helpers
{
    using System.Threading;
    using System.Threading.Tasks;

    using Newtonsoft.Json.Linq;

    using Rocket.Chat.Net.Interfaces;
    using Rocket.Chat.Net.Models;

    public abstract class DummyDdpClient : IDdpClient
    {
        public abstract void Dispose();

        public abstract string Url { get; }
        public abstract string SessionId { get; }
        public abstract bool IsDisposed { get; }

        public event DataReceived DataReceivedRaw;
        public event DdpReconnect DdpReconnect;

        public abstract Task PingAsync(CancellationToken token);

        public abstract Task ConnectAsync(CancellationToken token);

        public abstract Task<string> SubscribeAsync(string name, CancellationToken token, params object[] args);

        public abstract Task<string> SubscribeAndWaitAsync(string name, CancellationToken token, params object[] args);

        public abstract Task<JObject> CallAsync(string method, CancellationToken token, params object[] args);

        public void CallDataReceivedRaw(string type, JObject data)
        {
            DataReceivedRaw?.Invoke(type, data);
        }

        public void CallDdpReconnect()
        {
            DdpReconnect?.Invoke();
        }
    }
}