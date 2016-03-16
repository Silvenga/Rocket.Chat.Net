namespace Rocket.Chat.Net.Driver
{
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Dynamic;
    using System.Linq;

    using Rocket.Chat.Net.Helpers;

    public class StreamCollection
    {
        private readonly ConcurrentDictionary<string, IDictionary<string, object>> _collection =
            new ConcurrentDictionary<string, IDictionary<string, object>>();

        public string Name { get; set; }

        /// <summary>
        /// A collection item was changed.
        /// </summary>
        /// <param name="id">UUID of the changed object</param>
        /// <param name="fields">Value of object updated, will be merged into existing object</param>
        public void Changed(string id, object fields)
        {
            var fieldsDictionary = fields.AsDictionary();
            _collection.AddOrUpdate(id, fieldsDictionary,
                (existingId, existingField) => Combine(existingField, fieldsDictionary));
        }

        /// <summary>
        /// A collection item was added.
        /// </summary>
        /// <param name="id">UUID of the new object</param>
        /// <param name="fields">Value of object added, will override existing object</param>
        public void Added(string id, object fields)
        {
            var fieldsDictionary = fields.AsDictionary();
            _collection.AddOrUpdate(id, fieldsDictionary, (existingId, existingField) => fieldsDictionary);
        }

        /// <summary>
        /// A collection item was removed. 
        /// </summary>
        /// <param name="id">UUID of the removed object</param>
        public void Removed(string id)
        {
            IDictionary<string, object> value;
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
            IDictionary<string, object> result;
            var success = _collection.TryGetValue(id, out result);
            return success ? result as T : null;
        }

        /// <summary>
        /// Get object from collection as a dynamic type 
        /// </summary>
        /// <param name="id">UUID of the object</param>
        /// <returns>The object requested, null if object doesn't exist</returns>
        public dynamic GetDynamicById(string id)
        {
            IDictionary<string, object> result;
            var success = _collection.TryGetValue(id, out result);

            if (!success)
            {
                return null;
            }

            var eo = new ExpandoObject();
            var eoColl = (ICollection<KeyValuePair<string, object>>) eo;

            foreach (var kvp in result)
            {
                eoColl.Add(kvp);
            }

            return eo;
        }

        /// <summary>
        /// Get object from collection as a dynamic type
        /// </summary>
        /// <typeparam name="T">Anonymous type to create</typeparam>
        /// <param name="id">UUID of the object</param>
        /// <param name="type">Anonymous type to create</param>
        /// <returns>The object requested, null if object doesn't exist</returns>
        public T GetAnonymousTypeById<T>(string id, T type) where T : class
        {
            IDictionary<string, object> result;
            var success = _collection.TryGetValue(id, out result);

            return success ? result.ToAnonymousObject(type) : null;
        }

        /// <summary>
        /// Items of the collection
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IEnumerable<KeyValuePair<string, T>> Items<T>() where T : class
        {
            return _collection
                .ToList()
                .Select(value => new KeyValuePair<string, T>(value.Key, value.Value as T));
        }

        private static IDictionary<string, object> Combine(IDictionary<string, object> leftItem,
                                                           IDictionary<string, object> rightItem)
        {
            var result = new Dictionary<string, object>();

            foreach (var pair in leftItem.Concat(rightItem))
            {
                result[pair.Key] = pair.Value;
            }

            return result;
        }
    }
}