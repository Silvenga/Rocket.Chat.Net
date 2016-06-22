namespace Rocket.Chat.Net.Collections
{
    using System;
    using System.Collections.Generic;

    using Rocket.Chat.Net.Interfaces;

    public class TypedStreamCollection<T> where T : class
    {
        private readonly IStreamCollection _collection;

        public event EventHandler<TypedStreamCollectionEventArgs<T>> Modified;

        public string Name => _collection.Name;

        public TypedStreamCollection(IStreamCollection collection)
        {
            _collection = collection;
            _collection.Modified += OnModified;
        }

        public bool ContainsId(string id)
        {
            return _collection.ContainsId(id);
        }

        public T GetById(string id)
        {
            return _collection.GetById<T>(id);
        }

        public IEnumerable<KeyValuePair<string, T>> Items()
        {
            return _collection.Items<T>();
        }

        protected virtual void OnModified(object sender, StreamCollectionEventArgs streamCollectionEventArgs)
        {
            var result = streamCollectionEventArgs.To<T>();
            var modificationType = streamCollectionEventArgs.ModificationType;
            Modified?.Invoke(this, new TypedStreamCollectionEventArgs<T>(result, modificationType));
        }
    }
}