namespace HealthTrack.Application.Common.Models;

public class ApiResponse<T>
{
    public bool Success { get; init; }
    public T? Data { get; init; }
    public string? Message { get; init; }
    public List<string> Errors { get; init; } = [];

    public static ApiResponse<T> Ok(T? data, string? message = null)
        => new() { Success = true, Data = data, Message = message };

    public static ApiResponse<T> SuccessResponse(T data, string? message = null)
        => new() { Success = true, Data = data, Message = message };

    public static ApiResponse<T> Fail(string error)
        => new() { Success = false, Errors = [error] };

    public static ApiResponse<T> Fail(List<string> errors)
        => new() { Success = false, Errors = errors };

    public static ApiResponse<T> FailureResponse(string error)
        => new() { Success = false, Errors = [error] };

    public static ApiResponse<T> FailureResponse(List<string> errors)
        => new() { Success = false, Errors = errors };
}
