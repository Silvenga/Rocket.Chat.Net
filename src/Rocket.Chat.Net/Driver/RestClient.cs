using Newtonsoft.Json.Linq;
using Rocket.Chat.Net.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using RestSharp;
using NLog;
using WebSocketSharp;
using RestSharp.Authenticators;
using NLog.LayoutRenderers.Wrappers;
using Rocket.Chat.Net.Models.RestResults;
using System.IO;
using Microsoft.CodeAnalysis;
using System.Runtime.InteropServices.ComTypes;
using System.Net.Mime;
using NLog.LayoutRenderers;
using System.Linq;

namespace Rocket.Chat.Net.Driver
{
    public class RestClient : IRestClient
    {
        public RestSharp.RestClient _client;
        private IAuthenticator _authenticator;
        private bool _isLoggedIn;
        private string _instanceUrl;
        private bool _useSsl;
        ILogger _logger;

        public RestClient(string instanceUrl, bool useSsl, ILogger logger)
        {
            _logger = logger;
            _instanceUrl = instanceUrl;
            _useSsl = useSsl;
            _client = new RestSharp.RestClient((useSsl ? "https" : "http")  + "://" + instanceUrl + "/api/v1/");
        }

        private bool disposedValue;

        public string Url => throw new NotImplementedException();

        public bool IsDisposed => throw new NotImplementedException();

        public bool IsLoggedIn { get => _isLoggedIn; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="method">HTTP method, such as GET, POST, PUT etc.</param>
        /// <param name="path"></param>
        /// <param name="token"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public async Task<JObject> CallAsync(RestSharp.Method method, string path, CancellationToken token, params object[] args)
        {
            var request = new RestRequest(path, method);
            JArray data = JArray.FromObject(args);
            if (data != null)
            {
                request.AddBody(data.ToString());
            }
            var response = await _client.ExecuteAsync(request).ConfigureAwait(false);
            return JObject.Parse(response.Content);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="method">HTTP method, such as GET, POST, PUT etc.</param>
        /// <param name="path"></param>
        /// <param name="token"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public async Task<JObject> CallAsync(RestSharp.Method method, string path, CancellationToken token, object data)
        {
            var request = new RestRequest(path, method);
            if (data != null)
            {
                // TODO default serialization
                request.AddBody(JObject.FromObject(data).ToString());
            }
            var response = await _client.ExecuteAsync(request).ConfigureAwait(false);
            return JObject.Parse(response.Content);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="method">HTTP method, such as GET, POST, PUT etc.</param>
        /// <param name="path"></param>
        /// <param name="token"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public async Task<string> DownloadAsync(string path, CancellationToken token)
        {
            var request = new RestRequest(_useSsl ? "https" : "http" + "://" + _instanceUrl +  path, RestSharp.Method.Get);
            var response = await _client.ExecuteAsync(request).ConfigureAwait(false);
            if (response.RawBytes != null)
            {
                string filename = path.Split('/').Last();
                string filePath = Path.GetTempPath() + Path.GetRandomFileName() + Path.GetExtension(path);

                // Fallback with random file name
                while (System.IO.File.Exists(filePath))
                {
                    filePath = Path.GetTempPath() + Path.GetRandomFileName() + Path.GetExtension(filename);
                }

                using (var fs = new FileStream(filePath, FileMode.OpenOrCreate))
                {
                    fs.Write(response.RawBytes, 0, response.RawBytes.Length);
                }

                return filePath;
            }

            return null;
        }

        public async Task<JObject> UploadAsync(RestSharp.Method method, string path, CancellationToken token, params object[] args)
        {
            var request = new RestRequest(path, method);
            request.AlwaysMultipartFormData = true;
            for (int i = 0; i < args.Length; i++)
            {
                // TODO default serialization
                if (args[i] is string)
                    request.AddFile("file", args[i].ToString());
                else if (args[i] is FileStream)
                {
                    var fs = args[i] as FileStream;
                    request.AddFile("file", () => fs, fs.Name);
                }
            }
            var response = await _client.ExecuteAsync(request).ConfigureAwait(false);
            return JObject.Parse(response.Content);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: Verwalteten Zustand (verwaltete Objekte) bereinigen
                    _client.Dispose();
                }

                // TODO: Nicht verwaltete Ressourcen (nicht verwaltete Objekte) freigeben und Finalizer überschreiben
                // TODO: Große Felder auf NULL setzen
                disposedValue = true;
            }
        }

        // // TODO: Finalizer nur überschreiben, wenn "Dispose(bool disposing)" Code für die Freigabe nicht verwalteter Ressourcen enthält
        // ~RestClient()
        // {
        //     // Ändern Sie diesen Code nicht. Fügen Sie Bereinigungscode in der Methode "Dispose(bool disposing)" ein.
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Ändern Sie diesen Code nicht. Fügen Sie Bereinigungscode in der Methode "Dispose(bool disposing)" ein.
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        public async Task<RestResult> LoginAsync(object args)
        {
            var response = await CallAsync(Method.Post, "login", CancellationToken.None, args).ConfigureAwait(false);
            var result = response.ToObject<RestResult<RestLoginResult>>();
            if (result.Success)
            {
                string authToken = result.Data.AuthToken;
                string userId = result.Data.UserId;
                _client.Authenticator = new RocketAuthenticator(userId, authToken);
                _isLoggedIn = true;
            } else
            {
                _logger.Error($"Login Error: {result.Error}");
            }
            return result;
        }
    }

    public class RocketAuthenticator : IAuthenticator
    {
        private string authToken;
        private string userId;

        public RocketAuthenticator(string userId, string authToken) 
        {
            this.authToken = authToken;
            this.userId = userId;
        }

        public ValueTask Authenticate(RestSharp.RestClient client, RestRequest request)
        {
            request.AddHeader("X-Auth-Token", authToken);
            request.AddHeader("X-User-Id", userId);
            return new ValueTask();
        }
    }
}
