using FavouriteBooks.Api.Common;
using FavouriteBooks.Api.DTOs.Cart;

namespace FavouriteBooks.Api.Services;

public interface ICartService
{
    Task<Result<CartSummaryDto>> GetCartAsync(Guid? customerId, string? sessionId);
    Task<Result<CartSummaryDto>> AddItemAsync(AddCartItemRequest request);
    Task<Result<CartSummaryDto>> UpdateItemQuantityAsync(Guid bookId, UpdateCartItemRequest request);
    Task<Result<CartSummaryDto>> RemoveItemAsync(Guid? customerId, string? sessionId, Guid bookId);
    Task<Result> ClearCartAsync(Guid? customerId, string? sessionId);
}
