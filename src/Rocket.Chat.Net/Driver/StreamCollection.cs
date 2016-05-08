namespace Rocket.Chat.Net.Driver
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using Newtonsoft.Json.Serialization;

    using Rocket.Chat.Net.Interfaces;

    /// <summary>
    /// Collection containing objects taken from a DDP stream.
    /// </summary>
    public class StreamCollection : IStreamCollection
    {
        private readonly ConcurrentDictionary<string, JObject> _collection =
            new ConcurrentDictionary<string, JObject>();

        private readonly Func<JObject, JObject, JObject> _merge = (left, right) =>
        {
            var mergeSettings = new JsonMergeSettings
            {
                MergeArrayHandling = MergeArrayHandling.Union
            };

            left.Merge(right, mergeSettings);
            return left;
        };

        public StreamCollection(string name)
        {
            Name = name;
        }

        /// <summary>
        /// The name of the collection.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// A collection item was changed.
        /// Updates and merges existing item, when item doesn't exist create it
        /// </summary>
        /// <param name="id">UUID of the changed object</param>
        /// <param name="fields">Value of object updated, will be merged into existing object</param>
        public void Changed(string id, JObject fields)
        {
            _collection.AddOrUpdate(id, fields, (s, o) => _merge(o, fields));
        }

        /// <summary>
        /// A collection item was added.
        /// </summary>
        /// <param name="id">UUID of the new object</param>
        /// <param name="fields">Value of object added, will override existing object</param>
        public void Added(string id, JObject fields)
        {
            _collection.AddOrUpdate(id, fields, (existingId, existingField) => fields);
        }

        /// <summary>
        /// A collection item was removed. 
        /// </summary>
        /// <param name="id">UUID of the removed object</param>
        public void Removed(string id)
        {
            JObject value;
            _collection.TryRemove(id, out value);
        }

        /// <summary>
        /// Test if item exists in the collection.
        /// </summary>
        /// <param name="id">UUID of the item to search for</param>
        /// <returns>true if item exists</returns>
        public bool ContainsId(string id)
        {
            return _collection.ContainsKey(id);
        }

        /// <summary>
        /// Get object from collection
        /// </summary>
        /// <typeparam name="T">type to convert the object too</typeparam>
        /// <param name="id">UUID of the object</param>
        /// <returns>The object requested, null if object doesn't exist, null if object could not be created</returns>
        public T GetById<T>(string id) where T : class
        {
            JObject result;
            var success = _collection.TryGetValue(id, out result);
            return success ? result.ToObject<T>() : null;
        }

        /// <summary>
        /// Get object from collection
        /// </summary>
        /// <param name="id">UUID of the object</param>
        /// <returns>The object requested, null if object doesn't exist, null if object could not be created</returns>
        public JObject GetJObjectById(string id)
        {
            JObject result;
            var success = _collection.TryGetValue(id, out result);
            return success ? result : null;
        }

        /// <summary>
        /// Get object from collection as a JObject type
        /// </summary>
        /// <typeparam name="T">Anonymous type to create</typeparam>
        /// <param name="id">UUID of the object</param>
        /// <param name="type">Anonymous type to create</param>
        /// <returns>The object requested, null if object doesn't exist</returns>
        // ReSharper disable once UnusedParameter.Global
        public T GetAnonymousTypeById<T>(string id, T type) where T : class
        {
            JObject result;
            var success = _collection.TryGetValue(id, out result);

            return success ? result.ToObject<T>() : null;
        }

        /// <summary>
        /// Items of the collection
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IEnumerable<KeyValuePair<string, T>> Items<T>() where T : class
        {
            var settings = new JsonSerializerSettings()
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };

            var jsonSerializer = JsonSerializer.CreateDefault(settings);

            return _collection
                .ToList()
                .Select(value => new KeyValuePair<string, T>(value.Key, value.Value.ToObject<T>(jsonSerializer)));
        }
    }
}