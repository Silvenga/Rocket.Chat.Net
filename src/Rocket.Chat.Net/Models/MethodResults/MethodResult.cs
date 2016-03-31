namespace Rocket.Chat.Net.Models.MethodResults
{
    using System.Diagnostics.CodeAnalysis;

    using Newtonsoft.Json;

    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public class MethodResult<T>
    {
        [JsonProperty(PropertyName = "msg")]
        public string ResponseType { get; set; }

        public string Id { get; set; }

        public T Result { get; set; }

        [JsonProperty(PropertyName = "error")]
        public ErrorResult Error { get; set; }

        public bool HasError => Error != null;
    }

    public class MethodResult
    {
        [JsonProperty(PropertyName = "msg")]
        public string ResponseType { get; set; }

        public string Id { get; set; }

        [JsonProperty(PropertyName = "error")]
        public ErrorResult Error { get; set; }

        public bool HasError => Error != null;
    }
}