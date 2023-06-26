using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Rocket.Chat.Net.Models.RestResults;

namespace Rocket.Chat.Net.Interfaces
{
    public interface IRestClient : IDisposable 
    {
        string Url { get; }
        bool IsDisposed { get; }
        Task<JObject> CallAsync(RestSharp.Method method, string path, CancellationToken token, params object[] args);

        Task<string> DownloadAsync(string path, CancellationToken token);

        Task<JObject> UploadAsync(RestSharp.Method method, string path, CancellationToken token, params object[] args);
        Task<RestResult> LoginAsync(object args);

    }
}
