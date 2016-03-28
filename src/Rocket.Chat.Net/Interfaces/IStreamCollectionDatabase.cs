namespace Rocket.Chat.Net.Interfaces
{
    using System.Threading;
    using System.Threading.Tasks;

    using Rocket.Chat.Net.Driver;

    public interface IStreamCollectionDatabase
    {
        bool TryGetCollection(string collectionName, out IStreamCollection collection);
        IStreamCollection GetOrAddCollection(string collectionName);

        Task<IStreamCollection> WaitForCollectionAsync(string collectionName, string id,
                                                      CancellationToken token);
    }
}