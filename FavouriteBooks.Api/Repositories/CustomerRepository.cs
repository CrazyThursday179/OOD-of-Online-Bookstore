using FavouriteBooks.Api.Infrastructure;
using FavouriteBooks.Api.Models;

namespace FavouriteBooks.Api.Repositories;

public class CustomerRepository(JsonDataStore dataStore) : JsonRepositoryBase<Customer>(dataStore)
{
    protected override string FileName => "customers.json";
}
