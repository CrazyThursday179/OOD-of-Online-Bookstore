namespace FavouriteBooks.Api.Common;

public class Result
{
    public bool IsSuccess { get; init; }
    public string Message { get; init; } = string.Empty;
    public IReadOnlyList<string> Errors { get; init; } = [];

    public static Result Success(string message) => new()
    {
        IsSuccess = true,
        Message = message
    };

    public static Result Failure(string message, params string[] errors) => new()
    {
        IsSuccess = false,
        Message = message,
        Errors = errors
    };
}

public class Result<T> : Result
{
    public T? Data { get; init; }

    public static Result<T> Success(T data, string message) => new()
    {
        IsSuccess = true,
        Data = data,
        Message = message
    };

    public static new Result<T> Failure(string message, params string[] errors) => new()
    {
        IsSuccess = false,
        Message = message,
        Errors = errors
    };
}
