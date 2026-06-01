using FavouriteBooks.Api.Models;
using FavouriteBooks.Api.Models.Enums;

namespace FavouriteBooks.Api.DTOs.Orders;

public class OrderDto
{
    public Guid Id { get; set; }
    public Guid? CustomerId { get; set; }
    public string? GuestEmail { get; set; }
    public Guid ShipmentMethodId { get; set; }
    public Address DeliveryAddress { get; set; } = new();
    public IReadOnlyList<OrderItemDto> Items { get; set; } = [];
    public decimal Subtotal { get; set; }
    public decimal ShippingCost { get; set; }
    public decimal Total { get; set; }
    public Guid InvoiceId { get; set; }
    public Guid? ReceiptId { get; set; }
    public Guid? ShipmentId { get; set; }
    public OrderStatus Status { get; set; }
    public DateTime CreatedUtc { get; set; }
    public DateTime? PaidUtc { get; set; }
}
