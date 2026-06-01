using FavouriteBooks.Api.Infrastructure;
using FavouriteBooks.Api.Models;

namespace FavouriteBooks.Api.Repositories;

public class InvoiceRepository(JsonDataStore dataStore) : JsonRepositoryBase<Invoice>(dataStore)
{
    protected override string FileName => "invoices.json";
}
