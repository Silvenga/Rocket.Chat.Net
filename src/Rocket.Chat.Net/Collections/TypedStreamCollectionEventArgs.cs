namespace Rocket.Chat.Net.Collections
{
    using Rocket.Chat.Net.Models.Collections;

    public class TypedStreamCollectionEventArgs<T>
    {
        public T Result { get; set; }

        public ModificationType ModificationType { get; set; }

        public TypedStreamCollectionEventArgs(T result, ModificationType type)
        {
            Result = result;
            ModificationType = type;
        }
    }
}