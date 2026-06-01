using FavouriteBooks.Api.Infrastructure;
using FavouriteBooks.Api.Models;

namespace FavouriteBooks.Api.Repositories;

public class ShipmentMethodRepository(JsonDataStore dataStore) : JsonRepositoryBase<ShipmentMethod>(dataStore)
{
    protected override string FileName => "shipmentMethods.json";
}
