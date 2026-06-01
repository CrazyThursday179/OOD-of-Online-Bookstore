using FavouriteBooks.Api.Common;
using FavouriteBooks.Api.DTOs.Catalogue;

namespace FavouriteBooks.Api.Services;

public interface ICatalogueService
{
    Task<IReadOnlyList<BookDto>> GetBooksAsync(string? search, Guid? categoryId);
    Task<Result<BookDto>> GetBookByIdAsync(Guid id);
    Task<IReadOnlyList<BookCategoryDto>> GetCategoriesAsync();
}
