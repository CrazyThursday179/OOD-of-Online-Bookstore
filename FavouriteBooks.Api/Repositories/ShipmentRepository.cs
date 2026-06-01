using FavouriteBooks.Api.Infrastructure;
using FavouriteBooks.Api.Models;

namespace FavouriteBooks.Api.Repositories;

public class ShipmentRepository(JsonDataStore dataStore) : JsonRepositoryBase<Shipment>(dataStore)
{
    protected override string FileName => "shipments.json";
}
