using FavouriteBooks.Api.Infrastructure;
using FavouriteBooks.Api.Models;

namespace FavouriteBooks.Api.Repositories;

public class PaymentDetailsRepository(JsonDataStore dataStore) : JsonRepositoryBase<PaymentDetails>(dataStore)
{
    protected override string FileName => "payments.json";
}
