namespace Rocket.Chat.Net.Interfaces
{
    using System.Collections.Generic;

    using Newtonsoft.Json.Linq;

    public interface IStreamCollection
    {
        /// <summary>
        /// The name of the collection.
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// A collection item was changed.
        /// Updates and merges existing item, when item doesn't exist create it
        /// </summary>
        /// <param name="id">UUID of the changed object</param>
        /// <param name="fields">Value of object updated, will be merged into existing object</param>
        void Changed(string id, JObject fields);

        /// <summary>
        /// A collection item was added.
        /// </summary>
        /// <param name="id">UUID of the new object</param>
        /// <param name="fields">Value of object added, will override existing object</param>
        void Added(string id, JObject fields);

        /// <summary>
        /// A collection item was removed. 
        /// </summary>
        /// <param name="id">UUID of the removed object</param>
        void Removed(string id);

        /// <summary>
        /// Test if item exists in the collection.
        /// </summary>
        /// <param name="id">UUID of the item to search for</param>
        /// <returns>true if item exists</returns>
        bool ContainsId(string id);

        /// <summary>
        /// Get object from collection
        /// </summary>
        /// <typeparam name="T">type to convert the object too</typeparam>
        /// <param name="id">UUID of the object</param>
        /// <returns>The object requested, null if object doesn't exist, null if object could not be created</returns>
        T GetById<T>(string id) where T : class;

        /// <summary>
        /// Get object from collection as the given type
        /// </summary>
        /// <typeparam name="T">Anonymous type to create</typeparam>
        /// <param name="id">UUID of the object</param>
        /// <param name="type">Anonymous type to create</param>
        /// <returns>The object requested, null if object doesn't exist</returns>
        // ReSharper disable once UnusedParameter.Global
        T GetAnonymousTypeById<T>(string id, T type) where T : class;

        /// <summary>
        /// Items of the collection
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        IEnumerable<KeyValuePair<string, T>> Items<T>() where T : class;

        /// <summary>
        /// Get object from collection
        /// </summary>
        /// <param name="id">UUID of the object</param>
        /// <returns>The object requested, null if object doesn't exist, null if object could not be created</returns>
        JObject GetJObjectById(string id);
    }
}