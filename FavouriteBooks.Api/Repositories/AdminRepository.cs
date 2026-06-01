using FavouriteBooks.Api.Infrastructure;
using FavouriteBooks.Api.Models;

namespace FavouriteBooks.Api.Repositories;

public class AdminRepository(JsonDataStore dataStore) : JsonRepositoryBase<Admin>(dataStore)
{
    protected override string FileName => "admins.json";
}
