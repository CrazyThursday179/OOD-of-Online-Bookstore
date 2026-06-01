using FavouriteBooks.Api.Infrastructure;
using FavouriteBooks.Api.Models;

namespace FavouriteBooks.Api.Repositories;

public class ReceiptRepository(JsonDataStore dataStore) : JsonRepositoryBase<Receipt>(dataStore)
{
    protected override string FileName => "receipts.json";
}
