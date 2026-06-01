using FavouriteBooks.Api.DTOs.Cart;
using FavouriteBooks.Api.DTOs.Catalogue;
using FavouriteBooks.Api.DTOs.Orders;
using FavouriteBooks.Api.DTOs.Payments;
using FavouriteBooks.Api.DTOs.Shipments;
using FavouriteBooks.Api.Models;

namespace FavouriteBooks.Api.Services;

public static class ServiceMapper
{
    public static BookDto ToDto(this Book book) => new()
    {
        Id = book.Id,
        Isbn = book.Isbn,
        Title = book.Title,
        Author = book.Author,
        Description = book.Description,
        Format = book.Format,
        Price = book.Price,
        StockQuantity = book.StockQuantity,
        IsActive = book.IsActive,
        CategoryIds = book.CategoryIds
    };

    public static BookCategoryDto ToDto(this BookCategory category) => new()
    {
        Id = category.Id,
        Name = category.Name,
        Description = category.Description
    };

    public static CartSummaryDto ToDto(this ShoppingCart cart, IEnumerable<Book> books)
    {
        var bookMap = books.ToDictionary(book => book.Id);
        var items = cart.Items.Select(item =>
        {
            bookMap.TryGetValue(item.BookId, out var book);
            return new CartItemDto
            {
                BookId = item.BookId,
                BookTitle = book?.Title ?? "Unknown book",
                UnitPrice = item.UnitPriceSnapshot,
                Quantity = item.Quantity,
                LineTotal = item.UnitPriceSnapshot * item.Quantity
            };
        }).ToList();

        return new CartSummaryDto
        {
            CartId = cart.Id,
            CustomerId = cart.CustomerId,
            SessionId = cart.SessionId,
            Items = items,
            Subtotal = cart.CalculateTotal(),
            TotalQuantity = items.Sum(item => item.Quantity),
            LastUpdatedUtc = cart.LastUpdatedUtc
        };
    }

    public static OrderDto ToDto(this Order order) => new()
    {
        Id = order.Id,
        CustomerId = order.CustomerId,
        GuestEmail = order.GuestEmail,
        ShipmentMethodId = order.ShipmentMethodId,
        DeliveryAddress = order.DeliveryAddress,
        Items = order.Items.Select(item => new OrderItemDto
        {
            BookId = item.BookId,
            BookTitle = item.BookTitle,
            Quantity = item.Quantity,
            UnitPrice = item.UnitPrice,
            LineTotal = item.LineTotal
        }).ToList(),
        Subtotal = order.Subtotal,
        ShippingCost = order.ShippingCost,
        Total = order.Total,
        InvoiceId = order.InvoiceId,
        ReceiptId = order.ReceiptId,
        ShipmentId = order.ShipmentId,
        Status = order.Status,
        CreatedUtc = order.CreatedUtc,
        PaidUtc = order.PaidUtc
    };

    public static InvoiceDto ToDto(this Invoice invoice) => new()
    {
        Id = invoice.Id,
        OrderId = invoice.OrderId,
        InvoiceNumber = invoice.InvoiceNumber,
        IssuedUtc = invoice.IssuedUtc,
        DueUtc = invoice.DueUtc,
        Subtotal = invoice.Subtotal,
        ShippingCost = invoice.ShippingCost,
        Total = invoice.Total,
        PaymentStatus = invoice.PaymentStatus
    };

    public static ReceiptDto ToDto(this Receipt receipt) => new()
    {
        Id = receipt.Id,
        OrderId = receipt.OrderId,
        InvoiceId = receipt.InvoiceId,
        ReceiptNumber = receipt.ReceiptNumber,
        PaymentMethodType = receipt.PaymentMethodType,
        AmountPaid = receipt.AmountPaid,
        PaidUtc = receipt.PaidUtc
    };

    public static ShipmentDto ToDto(this Shipment shipment) => new()
    {
        Id = shipment.Id,
        OrderId = shipment.OrderId,
        ReceiptId = shipment.ReceiptId,
        ShipmentMethodId = shipment.ShipmentMethodId,
        DeliveryAddress = shipment.DeliveryAddress,
        Status = shipment.Status,
        TrackingCode = shipment.TrackingCode,
        CreatedUtc = shipment.CreatedUtc,
        DispatchedUtc = shipment.DispatchedUtc,
        DeliveredUtc = shipment.DeliveredUtc
    };

    public static ShipmentMethodDto ToDto(this ShipmentMethod method) => new()
    {
        Id = method.Id,
        Name = method.Name,
        Description = method.Description,
        Cost = method.Cost,
        EstimatedDeliveryDays = method.EstimatedDeliveryDays,
        IsActive = method.IsActive
    };
}
