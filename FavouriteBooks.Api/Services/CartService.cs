using FavouriteBooks.Api.Common;
using FavouriteBooks.Api.DTOs.Cart;
using FavouriteBooks.Api.Models;
using FavouriteBooks.Api.Repositories;

namespace FavouriteBooks.Api.Services;

public class CartService(
    CartRepository cartRepository,
    BookRepository bookRepository) : ICartService
{
    public async Task<Result<CartSummaryDto>> GetCartAsync(Guid? customerId, string? sessionId)
    {
        var ownerResult = CartOwner.Create(customerId, sessionId);
        if (!ownerResult.IsSuccess || ownerResult.Data is null)
        {
            return Result<CartSummaryDto>.Failure(ownerResult.Message, ownerResult.Errors.ToArray());
        }

        var cart = await GetOrCreateCartAsync(ownerResult.Data);
        var books = await bookRepository.GetAllAsync();

        return Result<CartSummaryDto>.Success(cart.ToDto(books), "Cart retrieved.");
    }

    public async Task<Result<CartSummaryDto>> AddItemAsync(AddCartItemRequest request)
    {
        if (request.Quantity <= 0)
        {
            return Result<CartSummaryDto>.Failure("Invalid quantity.", "Quantity must be positive.");
        }

        var ownerResult = CartOwner.Create(request.CustomerId, request.SessionId);
        if (!ownerResult.IsSuccess || ownerResult.Data is null)
        {
            return Result<CartSummaryDto>.Failure(ownerResult.Message, ownerResult.Errors.ToArray());
        }

        var books = await bookRepository.GetAllAsync();
        var book = books.FirstOrDefault(item => item.Id == request.BookId && item.IsActive);
        if (book is null)
        {
            return Result<CartSummaryDto>.Failure("Book not found.", "The selected book does not exist.");
        }

        if (!book.IsAvailable(request.Quantity))
        {
            return Result<CartSummaryDto>.Failure(
                "Insufficient stock.",
                $"Only {book.StockQuantity} units are currently available.");
        }

        var carts = await cartRepository.GetAllAsync();
        var cart = FindCart(carts, ownerResult.Data) ?? CreateCart(ownerResult.Data);

        try
        {
            cart.AddItem(book, request.Quantity);
        }
        catch (Exception ex) when (ex is ArgumentOutOfRangeException or InvalidOperationException)
        {
            return Result<CartSummaryDto>.Failure("Unable to add item to cart.", ex.Message);
        }

        UpsertCart(carts, cart);
        await cartRepository.SaveAllAsync(carts);

        return Result<CartSummaryDto>.Success(cart.ToDto(books), "Item added to cart.");
    }

    public async Task<Result<CartSummaryDto>> UpdateItemQuantityAsync(Guid bookId, UpdateCartItemRequest request)
    {
        if (request.Quantity <= 0)
        {
            return Result<CartSummaryDto>.Failure("Invalid quantity.", "Quantity must be positive.");
        }

        var ownerResult = CartOwner.Create(request.CustomerId, request.SessionId);
        if (!ownerResult.IsSuccess || ownerResult.Data is null)
        {
            return Result<CartSummaryDto>.Failure(ownerResult.Message, ownerResult.Errors.ToArray());
        }

        var books = await bookRepository.GetAllAsync();
        var book = books.FirstOrDefault(item => item.Id == bookId && item.IsActive);
        if (book is null)
        {
            return Result<CartSummaryDto>.Failure("Book not found.", "The selected book does not exist.");
        }

        if (!book.IsAvailable(request.Quantity))
        {
            return Result<CartSummaryDto>.Failure(
                "Insufficient stock.",
                $"Only {book.StockQuantity} units are currently available.");
        }

        var carts = await cartRepository.GetAllAsync();
        var cart = FindCart(carts, ownerResult.Data);
        if (cart is null)
        {
            return Result<CartSummaryDto>.Failure("Cart not found.", "No persisted cart exists for the supplied owner.");
        }

        try
        {
            cart.UpdateQuantity(book, request.Quantity);
        }
        catch (Exception ex) when (ex is ArgumentOutOfRangeException or InvalidOperationException)
        {
            return Result<CartSummaryDto>.Failure("Unable to update cart item.", ex.Message);
        }

        await cartRepository.SaveAllAsync(carts);
        return Result<CartSummaryDto>.Success(cart.ToDto(books), "Cart item quantity updated.");
    }

    public async Task<Result<CartSummaryDto>> RemoveItemAsync(Guid? customerId, string? sessionId, Guid bookId)
    {
        var ownerResult = CartOwner.Create(customerId, sessionId);
        if (!ownerResult.IsSuccess || ownerResult.Data is null)
        {
            return Result<CartSummaryDto>.Failure(ownerResult.Message, ownerResult.Errors.ToArray());
        }

        var carts = await cartRepository.GetAllAsync();
        var cart = FindCart(carts, ownerResult.Data);
        if (cart is null)
        {
            return Result<CartSummaryDto>.Failure("Cart not found.", "No persisted cart exists for the supplied owner.");
        }

        cart.RemoveItem(bookId);

        var books = await bookRepository.GetAllAsync();
        await cartRepository.SaveAllAsync(carts);

        return Result<CartSummaryDto>.Success(cart.ToDto(books), "Item removed from cart.");
    }

    public async Task<Result> ClearCartAsync(Guid? customerId, string? sessionId)
    {
        var ownerResult = CartOwner.Create(customerId, sessionId);
        if (!ownerResult.IsSuccess || ownerResult.Data is null)
        {
            return Result.Failure(ownerResult.Message, ownerResult.Errors.ToArray());
        }

        var carts = await cartRepository.GetAllAsync();
        var cart = FindCart(carts, ownerResult.Data);
        if (cart is null)
        {
            return Result.Failure("Cart not found.", "No persisted cart exists for the supplied owner.");
        }

        cart.Clear();
        await cartRepository.SaveAllAsync(carts);

        return Result.Success("Cart cleared.");
    }

    internal async Task<ShoppingCart> GetOrCreateCartAsync(CartOwner owner)
    {
        var carts = await cartRepository.GetAllAsync();
        var cart = FindCart(carts, owner);
        if (cart is not null)
        {
            return cart;
        }

        cart = CreateCart(owner);
        carts.Add(cart);
        await cartRepository.SaveAllAsync(carts);
        return cart;
    }

    private static ShoppingCart? FindCart(List<ShoppingCart> carts, CartOwner owner)
    {
        return carts.FirstOrDefault(cart =>
            cart.CustomerId == owner.CustomerId &&
            string.Equals(cart.SessionId, owner.SessionId, StringComparison.OrdinalIgnoreCase));
    }

    private static ShoppingCart CreateCart(CartOwner owner) => new()
    {
        Id = Guid.NewGuid(),
        CustomerId = owner.CustomerId,
        SessionId = owner.SessionId,
        LastUpdatedUtc = DateTime.UtcNow
    };

    private static void UpsertCart(List<ShoppingCart> carts, ShoppingCart cart)
    {
        var index = carts.FindIndex(item => item.Id == cart.Id);
        if (index >= 0)
        {
            carts[index] = cart;
        }
        else
        {
            carts.Add(cart);
        }
    }
}
