using FavouriteBooks.Api.Infrastructure;

namespace FavouriteBooks.Api.Repositories;

public abstract class JsonRepositoryBase<T>(JsonDataStore dataStore)
{
    protected JsonDataStore DataStore { get; } = dataStore;
    protected abstract string FileName { get; }

    public Task<List<T>> GetAllAsync() => DataStore.ReadCollectionAsync<T>(FileName);

    public Task SaveAllAsync(IEnumerable<T> items) => DataStore.WriteCollectionAsync(FileName, items);
}
