namespace Rocket.Chat.Net.Models.MethodResults
{
    using Newtonsoft.Json;

    public class CreateRoomResult
    {
        [JsonProperty(PropertyName = "rid")]
        public string RoomId { get; set; }
    }
}