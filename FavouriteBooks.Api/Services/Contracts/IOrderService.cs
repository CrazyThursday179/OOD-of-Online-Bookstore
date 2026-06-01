using FavouriteBooks.Api.Common;
using FavouriteBooks.Api.DTOs.Orders;

namespace FavouriteBooks.Api.Services;

public interface IOrderService
{
    Task<Result<CheckoutResponse>> CheckoutAsync(CheckoutRequest request);
    Task<Result<OrderDto>> GetOrderAsync(Guid orderId);
    Task<Result<InvoiceDto>> GetInvoiceAsync(Guid invoiceId);
    Task<IReadOnlyList<OrderDto>> GetAllOrdersAsync();
}
