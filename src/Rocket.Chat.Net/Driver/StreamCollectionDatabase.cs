namespace Rocket.Chat.Net.Driver
{
    using System;
    using System.Collections.Concurrent;
    using System.Threading;
    using System.Threading.Tasks;

    using Rocket.Chat.Net.Interfaces;

    public class StreamCollectionDatabase : IStreamCollectionDatabase
    {
        private readonly ConcurrentDictionary<string, StreamCollection> _collections =
            new ConcurrentDictionary<string, StreamCollection>();

        public bool TryGetCollection(string collectionName, out StreamCollection collection)
        {
            return _collections.TryGetValue(collectionName, out collection);
        }

        public StreamCollection GetOrAddCollection(string collectionName)
        {
            Func<string, StreamCollection> createCollection = name => new StreamCollection(name);

            return _collections.GetOrAdd(collectionName, createCollection);
        }

        public async Task<StreamCollection> WaitForCollectionAsync(string collectionName, string id,
                                                                   CancellationToken token)
        {
            return await Task.Run(() =>
            {
                while (true)
                {
                    StreamCollection collection;
                    var success = TryGetCollection(collectionName, out collection);

                    var collectonPopulated = success && collection.ContainsId(id);
                    if (collectonPopulated)
                    {
                        return collection;
                    }

                    token.ThrowIfCancellationRequested();
                    Thread.Sleep(10);
                }
            }, token);
        }
    }
}