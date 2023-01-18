using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Rocket.Chat.Net.Interfaces
{
    public interface IRestClient : IDisposable 
    {
        string Url { get; }
        bool IsDisposed { get; }
        Task<JObject> CallAsync(RestSharp.Method method, string path, CancellationToken token, params object[] args);
        Task LoginAsync(object args);

    }
}
