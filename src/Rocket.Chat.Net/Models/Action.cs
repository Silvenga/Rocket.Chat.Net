using JetBrains.Annotations;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rocket.Chat.Net.Models
{
    public class Action
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("msg_in_chat_window")]
        public bool MsgInChatWindow { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("msg")]
        public string Message { get; set; }
    }
}
