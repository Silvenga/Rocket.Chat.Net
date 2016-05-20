namespace Rocket.Chat.Net.JsonConverters
{
    using System;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    public class MeteorDateConverter : JsonConverter
    {
        private const string Key = "$date";

        public override bool CanConvert(Type objectType)
        {
            return typeof(DateTime?).IsAssignableFrom(objectType);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var dateTime = value as DateTime?;
            if (dateTime == null)
            {
                return;
            }

            writer.WriteStartObject();
            writer.WritePropertyName(Key);
            serializer.Serialize(writer, ToEpoch(dateTime.Value));
            writer.WriteEndObject();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
                                        JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
            {
                return null;
            }

            var jsonObject = JObject.Load(reader);
            var epoch = jsonObject[Key]?.ToObject<long>();
            if (epoch == null)
            {
                return null;
            }
            return FromEpoch(epoch.Value);
        }

        private static DateTime FromEpoch(long unixTime)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return epoch.AddMilliseconds(unixTime);
        }

        private static long ToEpoch(DateTime date)
        {
            var span = date.ToUniversalTime() - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return Convert.ToInt64(span.TotalMilliseconds);
        }
    }
}