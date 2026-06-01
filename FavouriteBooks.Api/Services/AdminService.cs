using FavouriteBooks.Api.Common;
using FavouriteBooks.Api.DTOs.Admin;
using FavouriteBooks.Api.DTOs.Catalogue;
using FavouriteBooks.Api.DTOs.Orders;
using FavouriteBooks.Api.DTOs.Shipments;
using FavouriteBooks.Api.Models;
using FavouriteBooks.Api.Models.Enums;
using FavouriteBooks.Api.Repositories;

namespace FavouriteBooks.Api.Services;

public class AdminService(
    BookRepository bookRepository,
    BookCategoryRepository categoryRepository,
    OrderRepository orderRepository,
    ShipmentRepository shipmentRepository) : IAdminService
{
    public async Task<Result<BookDto>> AddBookAsync(AdminBookRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Isbn) ||
            string.IsNullOrWhiteSpace(request.Title) ||
            string.IsNullOrWhiteSpace(request.Author))
        {
            return Result<BookDto>.Failure(
                "Missing required fields.",
                "ISBN, title, and author are required.");
        }

        if (request.Price < 0 || request.StockQuantity < 0)
        {
            return Result<BookDto>.Failure(
                "Invalid numeric values.",
                "Price and stock quantity must be zero or greater.");
        }

        var books = await bookRepository.GetAllAsync();
        if (books.Any(book => string.Equals(book.Isbn, request.Isbn.Trim(), StringComparison.OrdinalIgnoreCase)))
        {
            return Result<BookDto>.Failure("Duplicate ISBN.", "A book with the same ISBN already exists.");
        }

        var categories = await categoryRepository.GetAllAsync();
        var invalidCategoryIds = request.CategoryIds
            .Where(id => categories.All(category => category.Id != id))
            .ToList();

        if (invalidCategoryIds.Count > 0)
        {
            return Result<BookDto>.Failure(
                "Invalid category ids.",
                $"Unknown categories: {string.Join(", ", invalidCategoryIds)}");
        }

        var book = new Book
        {
            Id = Guid.NewGuid(),
            Isbn = request.Isbn.Trim(),
            Title = request.Title.Trim(),
            Author = request.Author.Trim(),
            Description = request.Description.Trim(),
            Format = request.Format.Trim(),
            Price = request.Price,
            StockQuantity = request.StockQuantity,
            IsActive = true,
            CategoryIds = request.CategoryIds
        };

        books.Add(book);
        await bookRepository.SaveAllAsync(books);

        return Result<BookDto>.Success(book.ToDto(), "Book created.");
    }

    public async Task<Result<BookDto>> UpdateBookStockAsync(Guid bookId, UpdateBookStockRequest request)
    {
        if (request.StockQuantity < 0)
        {
            return Result<BookDto>.Failure("Invalid stock quantity.", "Stock quantity must be zero or greater.");
        }

        var books = await bookRepository.GetAllAsync();
        var book = books.FirstOrDefault(item => item.Id == bookId);
        if (book is null)
        {
            return Result<BookDto>.Failure("Book not found.", $"No book exists for id '{bookId}'.");
        }

        book.SetStockQuantity(request.StockQuantity);
        await bookRepository.SaveAllAsync(books);

        return Result<BookDto>.Success(book.ToDto(), "Book stock updated.");
    }

    public async Task<IReadOnlyList<OrderDto>> GetOrdersAsync()
    {
        var orders = await orderRepository.GetAllAsync();
        return orders
            .OrderByDescending(order => order.CreatedUtc)
            .Select(order => order.ToDto())
            .ToList();
    }

    public async Task<Result<ShipmentDto>> UpdateShipmentStatusAsync(Guid shipmentId, UpdateShipmentStatusRequest request)
    {
        var shipments = await shipmentRepository.GetAllAsync();
        var shipment = shipments.FirstOrDefault(item => item.Id == shipmentId);
        if (shipment is null)
        {
            return Result<ShipmentDto>.Failure("Shipment not found.", $"No shipment exists for id '{shipmentId}'.");
        }

        try
        {
            switch (request.Status)
            {
                case ShipmentStatus.ReadyForDispatch:
                case ShipmentStatus.Pending:
                    shipment.Status = request.Status;
                    break;
                case ShipmentStatus.Dispatched:
                    shipment.MarkDispatched();
                    break;
                case ShipmentStatus.Delivered:
                    shipment.MarkDelivered();
                    break;
                default:
                    return Result<ShipmentDto>.Failure("Invalid shipment status.", "Unsupported shipment status.");
            }
        }
        catch (InvalidOperationException ex)
        {
            return Result<ShipmentDto>.Failure("Unable to update shipment status.", ex.Message);
        }

        await shipmentRepository.SaveAllAsync(shipments);
        return Result<ShipmentDto>.Success(shipment.ToDto(), "Shipment status updated.");
    }
}
