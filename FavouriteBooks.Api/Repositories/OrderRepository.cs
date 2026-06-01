using FavouriteBooks.Api.Infrastructure;
using FavouriteBooks.Api.Models;

namespace FavouriteBooks.Api.Repositories;

public class OrderRepository(JsonDataStore dataStore) : JsonRepositoryBase<Order>(dataStore)
{
    protected override string FileName => "orders.json";
}
