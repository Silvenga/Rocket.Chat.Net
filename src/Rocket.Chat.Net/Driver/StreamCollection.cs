namespace Rocket.Chat.Net.Driver
{
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;

    public class StreamCollection
    {
        private readonly ConcurrentDictionary<string, object> _collection = new ConcurrentDictionary<string, object>();

        public string Name { get; set; }

        public void Changed(string id, object fields)
        {
            _collection.AddOrUpdate(id, fields, (existingId, existingField) => fields);
        }

        public void Added(string id, object fields)
        {
            _collection.AddOrUpdate(id, fields, (existingId, existingField) => existingField);
        }

        public void Removed(string id)
        {
            object value;
            _collection.TryRemove(id, out value);
        }

        public IEnumerable<KeyValuePair<string, T>> Enumerator<T>() where T : class
        {
            return _collection
                .ToList()
                .Select(value => new KeyValuePair<string, T>(value.Key, value.Value as T));
        }
    }
}