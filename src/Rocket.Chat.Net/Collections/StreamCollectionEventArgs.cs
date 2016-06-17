namespace Rocket.Chat.Net.Collections
{
    using Newtonsoft.Json.Linq;

    public class StreamCollectionEventArgs
    {
        public JObject Result { get; set; }

        public ModificationType ModificationType { get; set; }

        public StreamCollectionEventArgs(JObject result, ModificationType type)
        {
            Result = result;
            ModificationType = type;
        }

        public T To<T>()
        {
            return Result.ToObject<T>();
        }
    }
}