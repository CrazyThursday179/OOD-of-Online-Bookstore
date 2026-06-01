using FavouriteBooks.Api.Common;
using FavouriteBooks.Api.DTOs.Catalogue;
using FavouriteBooks.Api.Repositories;

namespace FavouriteBooks.Api.Services;

public class CatalogueService(
    BookRepository bookRepository,
    BookCategoryRepository categoryRepository) : ICatalogueService
{
    public async Task<IReadOnlyList<BookDto>> GetBooksAsync(string? search, Guid? categoryId)
    {
        var books = await bookRepository.GetAllAsync();

        var query = books.Where(book => book.IsActive);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            query = query.Where(book =>
                book.Title.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                book.Author.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                book.Isbn.Contains(term, StringComparison.OrdinalIgnoreCase));
        }

        if (categoryId.HasValue)
        {
            query = query.Where(book => book.CategoryIds.Contains(categoryId.Value));
        }

        return query
            .OrderBy(book => book.Title)
            .Select(book => book.ToDto())
            .ToList();
    }

    public async Task<Result<BookDto>> GetBookByIdAsync(Guid id)
    {
        var books = await bookRepository.GetAllAsync();
        var book = books.FirstOrDefault(item => item.Id == id && item.IsActive);

        return book is null
            ? Result<BookDto>.Failure("Book not found.", $"No active book exists for id '{id}'.")
            : Result<BookDto>.Success(book.ToDto(), "Book retrieved.");
    }

    public async Task<IReadOnlyList<BookCategoryDto>> GetCategoriesAsync()
    {
        var categories = await categoryRepository.GetAllAsync();
        return categories
            .OrderBy(category => category.Name)
            .Select(category => category.ToDto())
            .ToList();
    }
}
