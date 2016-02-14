namespace Rocket.Chat.Net.Models
{
    public delegate void DataReceived(string type, dynamic data);
    public delegate void MessageReceived(RocketMessage rocketMessage);
}
