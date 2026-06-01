using FavouriteBooks.Api.Infrastructure;
using FavouriteBooks.Api.Models;

namespace FavouriteBooks.Api.Repositories;

public class BookRepository(JsonDataStore dataStore) : JsonRepositoryBase<Book>(dataStore)
{
    protected override string FileName => "books.json";
}
