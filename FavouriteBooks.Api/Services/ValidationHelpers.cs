using FavouriteBooks.Api.Models;

namespace FavouriteBooks.Api.Services;

public static class ValidationHelpers
{
    public static IReadOnlyList<string> ValidateAddress(Address address) => address.GetValidationErrors();
}
