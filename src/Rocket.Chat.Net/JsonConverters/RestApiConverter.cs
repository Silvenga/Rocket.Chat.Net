using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rocket.Chat.Net.JsonConverters
{
    public class RestApiConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            if (objectType == typeof(string)) 
                return true;
            else 
                return false;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var value = reader.Value;
            if (value == null || String.IsNullOrWhiteSpace(value.ToString()))
            {
                return false;
            }

            if ("success".Equals(value.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            return false;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
