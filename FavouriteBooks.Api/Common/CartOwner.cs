namespace FavouriteBooks.Api.Common;

public class CartOwner
{
    public Guid? CustomerId { get; init; }
    public string? SessionId { get; init; }

    public static Result<CartOwner> Create(Guid? customerId, string? sessionId)
    {
        if (customerId is null && string.IsNullOrWhiteSpace(sessionId))
        {
            return Result<CartOwner>.Failure(
                "Cart owner is required.",
                "Either customerId or sessionId must be supplied.");
        }

        return Result<CartOwner>.Success(
            new CartOwner
            {
                CustomerId = customerId,
                SessionId = string.IsNullOrWhiteSpace(sessionId) ? null : sessionId.Trim()
            },
            "Cart owner resolved.");
    }
}
