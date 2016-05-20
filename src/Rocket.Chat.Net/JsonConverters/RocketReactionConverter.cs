namespace Rocket.Chat.Net.JsonConverters
{
    using System;
    using System.Collections.Generic;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    using Rocket.Chat.Net.Models;

    public class RocketReactionConverter : JsonConverter
    {
        private const string UsernameKey = "usernames";

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var reactions = value as IList<Reaction>;
            if (reactions == null)
            {
                return;
            }

            writer.WriteStartObject();
            foreach (var reaction in reactions)
            {
                writer.WritePropertyName(reaction.Name);
                {
                    writer.WriteStartObject();
                    writer.WritePropertyName(UsernameKey);
                    serializer.Serialize(writer, reaction.Usernames);
                    writer.WriteEndObject();
                }
            }
            writer.WriteEndObject();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
                                        JsonSerializer serializer)
        {
            IList<Reaction> reactions = new List<Reaction>();
            if (reader.TokenType == JsonToken.Null)
            {
                return reactions;
            }

            var jsonObject = JObject.Load(reader);
            foreach (var o in jsonObject)
            {
                var name = o.Key;
                var usernames = o.Value[UsernameKey]
                    .ToObject<List<string>>();

                var reaction = new Reaction
                {
                    Name = name,
                    Usernames = usernames
                };
                reactions.Add(reaction);
            }

            return reactions;
        }

        public override bool CanConvert(Type objectType)
        {
            throw new NotImplementedException();
        }
    }
}