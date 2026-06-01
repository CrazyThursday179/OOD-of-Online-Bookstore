using FavouriteBooks.Api.Infrastructure;
using FavouriteBooks.Api.Models;

namespace FavouriteBooks.Api.Repositories;

public class CartRepository(JsonDataStore dataStore) : JsonRepositoryBase<ShoppingCart>(dataStore)
{
    protected override string FileName => "carts.json";
}
