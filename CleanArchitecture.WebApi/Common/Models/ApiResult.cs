namespace CleanArchitecture.WebApi.Common.Models;

public class ApiResult<T>
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public T? Data { get; set; }

    public static ApiResult<T> SuccessResult(T data, string? message = null)
    {
        return new ApiResult<T> { Success = true, Data = data, Message = message };
    }

    public static ApiResult<T> Fail(string message)
    {
        return new ApiResult<T> { Success = false, Message = message };
    }
}
