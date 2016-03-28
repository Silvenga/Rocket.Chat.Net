namespace Rocket.Chat.Net.Models
{
    using Newtonsoft.Json.Linq;

    public delegate void DataReceived(string type, JObject data);

    public delegate void MessageReceived(RocketMessage rocketMessage);

    public delegate void DdpReconnect();
}