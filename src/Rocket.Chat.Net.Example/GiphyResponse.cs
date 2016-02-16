namespace Rocket.Chat.Net.Example
{
    using System.Collections.Generic;

    using RestSharp;

    using Rocket.Chat.Net.Helpers;
    using Rocket.Chat.Net.Interfaces;
    using Rocket.Chat.Net.Models;

    internal class GiphyResponse : IBotResponse
    {
        private const string GiphyCommand = "giphy";
        private const string GiphyApiKey = "dc6zaTOxFJmzC";
        private const string Rating = "pg-13";
        private readonly RestClient _client = new RestClient("http://api.giphy.com/");

        public IEnumerable<BasicResponse> Response(RocketMessage message)
        {
            if (message.Message.StartsWith(GiphyCommand) && !message.Message.Equals(GiphyCommand))
            {
                var search = message.Message.Replace(GiphyCommand, "").Trim();
                var url = GetGiphy(search);
                yield return message.CreateBasicReply(url);
            }
        }

        private string GetGiphy(string search)
        {
            var request = new RestRequest("/v1/gifs/translate", Method.GET);
            request.AddQueryParameter("s", search);
            request.AddQueryParameter("rating", Rating);
            request.AddQueryParameter("api_key", GiphyApiKey);

            var response = _client.Execute<dynamic>(request);

            var url = response.Data["data"]["images"]["fixed_height"]["url"];

            return url;
        }
    }
}