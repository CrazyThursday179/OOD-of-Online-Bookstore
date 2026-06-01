using FavouriteBooks.Api.Infrastructure;
using FavouriteBooks.Api.Models;

namespace FavouriteBooks.Api.Repositories;

public class BookCategoryRepository(JsonDataStore dataStore) : JsonRepositoryBase<BookCategory>(dataStore)
{
    protected override string FileName => "categories.json";
}
