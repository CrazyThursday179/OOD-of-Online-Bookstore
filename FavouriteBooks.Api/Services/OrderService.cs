using FavouriteBooks.Api.Common;
using FavouriteBooks.Api.DTOs.Orders;
using FavouriteBooks.Api.Models;
using FavouriteBooks.Api.Models.Enums;
using FavouriteBooks.Api.Repositories;

namespace FavouriteBooks.Api.Services;

public class OrderService(
    CartRepository cartRepository,
    BookRepository bookRepository,
    CustomerRepository customerRepository,
    ShipmentMethodRepository shipmentMethodRepository,
    OrderRepository orderRepository,
    InvoiceRepository invoiceRepository) : IOrderService
{
    public async Task<Result<CheckoutResponse>> CheckoutAsync(CheckoutRequest request)
    {
        var ownerResult = CartOwner.Create(request.CustomerId, request.SessionId);
        if (!ownerResult.IsSuccess || ownerResult.Data is null)
        {
            return Result<CheckoutResponse>.Failure(ownerResult.Message, ownerResult.Errors.ToArray());
        }

        if (!request.DeliveryAddress.IsValid())
        {
            return Result<CheckoutResponse>.Failure(
                "Invalid delivery address.",
                request.DeliveryAddress.GetValidationErrors().ToArray());
        }

        if (request.CustomerId is null && string.IsNullOrWhiteSpace(request.GuestEmail))
        {
            return Result<CheckoutResponse>.Failure(
                "Guest email is required.",
                "Guest checkout must include guestEmail.");
        }

        var shipmentMethods = await shipmentMethodRepository.GetAllAsync();
        var shipmentMethod = shipmentMethods.FirstOrDefault(item => item.Id == request.ShipmentMethodId && item.IsActive);
        if (shipmentMethod is null)
        {
            return Result<CheckoutResponse>.Failure("Shipment method not found.", "A valid shipment method is required.");
        }

        if (request.CustomerId.HasValue)
        {
            var customers = await customerRepository.GetAllAsync();
            var customer = customers.FirstOrDefault(item => item.Id == request.CustomerId.Value);
            if (customer is null)
            {
                return Result<CheckoutResponse>.Failure("Customer not found.", "The provided customerId does not exist.");
            }
        }

        var carts = await cartRepository.GetAllAsync();
        var cart = carts.FirstOrDefault(item =>
            item.CustomerId == ownerResult.Data.CustomerId &&
            string.Equals(item.SessionId, ownerResult.Data.SessionId, StringComparison.OrdinalIgnoreCase));

        if (cart is null || cart.IsEmpty())
        {
            return Result<CheckoutResponse>.Failure("Cart is empty.", "Cart must contain at least one item before checkout.");
        }

        var books = await bookRepository.GetAllAsync();
        var orderItems = new List<OrderItem>();

        foreach (var cartItem in cart.Items)
        {
            var book = books.FirstOrDefault(item => item.Id == cartItem.BookId && item.IsActive);
            if (book is null)
            {
                return Result<CheckoutResponse>.Failure("Book not found.", $"Book '{cartItem.BookId}' no longer exists.");
            }

            if (cartItem.Quantity <= 0)
            {
                return Result<CheckoutResponse>.Failure("Invalid cart quantity.", "Cart quantities must be positive.");
            }

            if (!book.IsAvailable(cartItem.Quantity))
            {
                return Result<CheckoutResponse>.Failure(
                    "Insufficient stock.",
                    $"'{book.Title}' has only {book.StockQuantity} units remaining.");
            }

            orderItems.Add(new OrderItem
            {
                BookId = book.Id,
                BookTitle = book.Title,
                Quantity = cartItem.Quantity,
                UnitPrice = book.Price
            });
        }

        var subtotal = orderItems.Sum(item => item.LineTotal);
        var invoice = new Invoice
        {
            Id = Guid.NewGuid(),
            InvoiceNumber = $"INV-{DateTime.UtcNow:yyyyMMdd}-{Random.Shared.Next(1000, 9999)}",
            IssuedUtc = DateTime.UtcNow,
            DueUtc = DateTime.UtcNow.AddDays(7),
            ShipmentMethodId = shipmentMethod.Id,
            Subtotal = subtotal,
            ShippingCost = shipmentMethod.Cost,
            Total = subtotal + shipmentMethod.Cost,
            PaymentStatus = PaymentStatus.Pending
        };

        var order = new Order
        {
            Id = Guid.NewGuid(),
            CustomerId = request.CustomerId,
            GuestEmail = request.CustomerId is null ? request.GuestEmail?.Trim() : null,
            ShipmentMethodId = shipmentMethod.Id,
            DeliveryAddress = request.DeliveryAddress,
            Items = orderItems,
            Subtotal = invoice.Subtotal,
            ShippingCost = invoice.ShippingCost,
            Total = invoice.Total,
            InvoiceId = invoice.Id,
            Status = OrderStatus.PendingPayment,
            CreatedUtc = DateTime.UtcNow
        };

        invoice.OrderId = order.Id;

        foreach (var item in orderItems)
        {
            var book = books.First(book => book.Id == item.BookId);
            book.ReduceStock(item.Quantity);
        }

        cart.Clear();

        var orders = await orderRepository.GetAllAsync();
        var invoices = await invoiceRepository.GetAllAsync();

        orders.Add(order);
        invoices.Add(invoice);

        await orderRepository.SaveAllAsync(orders);
        await invoiceRepository.SaveAllAsync(invoices);
        await bookRepository.SaveAllAsync(books);
        await cartRepository.SaveAllAsync(carts);

        return Result<CheckoutResponse>.Success(
            new CheckoutResponse
            {
                Order = order.ToDto(),
                Invoice = invoice.ToDto()
            },
            "Checkout complete. Invoice generated and order is awaiting payment.");
    }

    public async Task<Result<OrderDto>> GetOrderAsync(Guid orderId)
    {
        var orders = await orderRepository.GetAllAsync();
        var order = orders.FirstOrDefault(item => item.Id == orderId);

        return order is null
            ? Result<OrderDto>.Failure("Order not found.", $"No order exists for id '{orderId}'.")
            : Result<OrderDto>.Success(order.ToDto(), "Order retrieved.");
    }

    public async Task<Result<InvoiceDto>> GetInvoiceAsync(Guid invoiceId)
    {
        var invoices = await invoiceRepository.GetAllAsync();
        var invoice = invoices.FirstOrDefault(item => item.Id == invoiceId);

        return invoice is null
            ? Result<InvoiceDto>.Failure("Invoice not found.", $"No invoice exists for id '{invoiceId}'.")
            : Result<InvoiceDto>.Success(invoice.ToDto(), "Invoice retrieved.");
    }

    public async Task<IReadOnlyList<OrderDto>> GetAllOrdersAsync()
    {
        var orders = await orderRepository.GetAllAsync();
        return orders
            .OrderByDescending(order => order.CreatedUtc)
            .Select(order => order.ToDto())
            .ToList();
    }
}
