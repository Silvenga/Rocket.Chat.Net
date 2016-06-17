namespace Rocket.Chat.Net.Collections
{
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