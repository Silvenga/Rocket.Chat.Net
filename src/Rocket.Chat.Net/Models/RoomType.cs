namespace Rocket.Chat.Net.Models
{
    using System.Runtime.Serialization;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    [JsonConverter(typeof(StringEnumConverter))]
    public enum RoomType
    {
        [EnumMember(Value = "p")] PrivateGroup = 'p',
        [EnumMember(Value = "g")] PrivateGroup2 = 'g',
        [EnumMember(Value = "d")] DirectMessage = 'd',
        [EnumMember(Value = "c")] Channel = 'c'
    }
}