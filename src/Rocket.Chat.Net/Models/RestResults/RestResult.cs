using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Newtonsoft.Json;
using Rocket.Chat.Net.JsonConverters;

namespace Rocket.Chat.Net.Models.RestResults
{
    public class RestResult
    {
        [JsonProperty(PropertyName = "_id")]
        public string Id { get; set; }

        [JsonConverter(typeof(RestApiConverter))]
        public bool Success { get; set; }

        public string Error { get; set; }
    }

    public class RestResult<T> : RestResult
    {
        public T Data { get; set; }
    }
}