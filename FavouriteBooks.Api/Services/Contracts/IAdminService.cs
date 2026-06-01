using FavouriteBooks.Api.Common;
using FavouriteBooks.Api.DTOs.Admin;
using FavouriteBooks.Api.DTOs.Catalogue;
using FavouriteBooks.Api.DTOs.Orders;
using FavouriteBooks.Api.DTOs.Shipments;

namespace FavouriteBooks.Api.Services;

public interface IAdminService
{
    Task<Result<BookDto>> AddBookAsync(AdminBookRequest request);
    Task<Result<BookDto>> UpdateBookStockAsync(Guid bookId, UpdateBookStockRequest request);
    Task<IReadOnlyList<OrderDto>> GetOrdersAsync();
    Task<Result<ShipmentDto>> UpdateShipmentStatusAsync(Guid shipmentId, UpdateShipmentStatusRequest request);
}
