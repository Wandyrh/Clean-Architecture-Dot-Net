using CleanArchitecture.Application.Common.Exceptions;
using CleanArchitecture.WebApi.Common.Models;
using FluentValidation;
using Newtonsoft.Json;
using System.Net;

namespace CleanArchitecture.WebApi.Middlewares;

public class GlobalExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;

    public GlobalExceptionHandlerMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlerMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await _next(httpContext);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(httpContext, ex);
        }
    }

    private Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var apiResult = new ApiResult<string>
        {
            Success = false,
            Data = null
        };

        context.Response.ContentType = "application/json";
        apiResult.Message = exception.Message;

        switch (exception)
        {
            case ArgumentException:
            case InvalidOperationException:
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                break;
            case ValidationException:
            case IDMismatchException:
            case InvalidLoginException:
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                break;
            case NotFoundException:
            case KeyNotFoundException:
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                break;
            case UnauthorizedAccessException:
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                break;  
            case TimeoutException:
                context.Response.StatusCode = (int)HttpStatusCode.GatewayTimeout;
                apiResult.Message = "Request timed out";
                break;
            case NotImplementedException:
                context.Response.StatusCode = (int)HttpStatusCode.NotImplemented;
                apiResult.Message = "Not implemented";
                break;
            case FormatException:
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                apiResult.Message = "Invalid format";
                break;
            case OperationCanceledException:
                context.Response.StatusCode = (int)HttpStatusCode.RequestTimeout;
                apiResult.Message = "Request was cancelled";
                break;
            case var ex when exception.GetType().Name == "DbUpdateException":
                context.Response.StatusCode = (int)HttpStatusCode.Conflict;
                apiResult.Message = "Database update error";
                break;
            default:
                apiResult.Message = "Internal Server Error";
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                break;
        }

        _logger.LogError(exception, "An error has occurred: {Message}", exception.Message);

        var settings = new JsonSerializerSettings
        {
            ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver()
        };
        var exceptionResult = JsonConvert.SerializeObject(apiResult, settings);
        return context.Response.WriteAsync(exceptionResult);
    }
}
