namespace Rocket.Chat.Net.Helpers
{
    using System;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    public class MeteorDateSerializer : JsonConverter
    {
        private const string Key = "$date";

        /// <summary>
        /// Determines whether this instance can convert the specified object type.
        /// </summary>
        /// <param name="objectType">Type of the object.</param>
        /// <returns>
        /// <c>true</c> if this instance can convert the specified object type; otherwise, <c>false</c>.
        /// </returns>
        public override bool CanConvert(Type objectType)
        {
            return typeof(DateTime?).IsAssignableFrom(objectType);
        }

        /// <summary>
        /// Writes the JSON representation of the object.
        /// </summary>
        /// <param name="writer">The <see cref="T:Newtonsoft.Json.JsonWriter"/> to write to.</param><param name="value">The value.</param><param name="serializer">The calling serializer.</param>
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

        /// <summary>
        /// Reads the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T:Newtonsoft.Json.JsonReader"/> to read from.</param><param name="objectType">Type of the object.</param><param name="existingValue">The existing value of object being read.</param><param name="serializer">The calling serializer.</param>
        /// <returns>
        /// The object value.
        /// </returns>
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