namespace Rocket.Chat.Net.Models.Results
{
    using Newtonsoft.Json;

    public class CreateRoomResult
    {
        [JsonProperty(PropertyName = "rid")]
        public string RoomId { get; set; }
    }
}