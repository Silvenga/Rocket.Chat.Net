namespace Rocket.Chat.Net.Interfaces
{
    using System.Threading;
    using System.Threading.Tasks;

    using Rocket.Chat.Net.Driver;

    public interface IStreamCollectionDatabase
    {
        bool TryGetCollection(string collectionName, out StreamCollection collection);
        StreamCollection GetOrAddCollection(string collectionName);

        Task<StreamCollection> WaitForCollectionAsync(string collectionName, string id,
                                                CancellationToken token);
    }
}