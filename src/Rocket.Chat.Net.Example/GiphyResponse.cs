namespace Rocket.Chat.Net.Example
{
    using System.Collections.Generic;

    using RestSharp;

    using Rocket.Chat.Net.Bot;
    using Rocket.Chat.Net.Bot.Helpers;
    using Rocket.Chat.Net.Bot.Interfaces;
    using Rocket.Chat.Net.Bot.Models;
    using Rocket.Chat.Net.Models;

    internal class GiphyResponse : IBotResponse
    {
        private const string GiphyCommand = "giphy";
        private const string GiphyApiKey = "dc6zaTOxFJmzC";
        private const string Rating = "pg-13";
        private readonly RestClient _client = new RestClient("http://api.giphy.com/");

        public bool CanRespond(ResponseContext context)
        {
            var message = context.Message;
            return message.Message.StartsWith(GiphyCommand) && !message.Message.Equals(GiphyCommand);
        }

        public IEnumerable<IMessageResponse> GetResponse(ResponseContext context, RocketChatBot caller)
        {
            var message = context.Message;

            var search = message.Message.Replace(GiphyCommand, "").Trim();
            var url = GetGiphy(search);
            var attachment = new Attachment
            {
                ImageUrl = url
            };
            yield return message.CreateAttachmentReply(attachment);
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