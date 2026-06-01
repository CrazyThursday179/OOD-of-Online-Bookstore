using FavouriteBooks.Api.Models.Enums;

namespace FavouriteBooks.Api.Models;

public class Order
{
    public Guid Id { get; set; }
    public Guid? CustomerId { get; set; }
    public string? GuestEmail { get; set; }
    public Guid ShipmentMethodId { get; set; }
    public Address DeliveryAddress { get; set; } = new();
    public List<OrderItem> Items { get; set; } = [];
    public decimal Subtotal { get; set; }
    public decimal ShippingCost { get; set; }
    public decimal Total { get; set; }
    public Guid InvoiceId { get; set; }
    public Guid? ReceiptId { get; set; }
    public Guid? ShipmentId { get; set; }
    public OrderStatus Status { get; set; }
    public DateTime CreatedUtc { get; set; }
    public DateTime? PaidUtc { get; set; }

    public void MarkPaymentFailed()
    {
        Status = OrderStatus.PaymentFailed;
    }

    public void MarkPaid()
    {
        Status = OrderStatus.Paid;
        PaidUtc = DateTime.UtcNow;
    }

    public void AttachReceipt(Guid receiptId)
    {
        ReceiptId = receiptId;
    }

    public void AttachShipment(Guid shipmentId)
    {
        ShipmentId = shipmentId;
        Status = OrderStatus.ShipmentCreated;
    }
}
