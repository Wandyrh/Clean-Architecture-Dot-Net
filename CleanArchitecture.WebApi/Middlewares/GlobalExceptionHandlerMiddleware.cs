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

        var statusCode = HttpStatusCode.InternalServerError;
        var logLevel = LogLevel.Error;

        switch (exception)
        {
            case ArgumentException:
            case InvalidOperationException:
                statusCode = HttpStatusCode.InternalServerError;
                logLevel = LogLevel.Error;
                break;
            case ValidationException validationEx:
                statusCode = HttpStatusCode.BadRequest;
                logLevel = LogLevel.Warning;
                _logger.LogWarning(validationEx, 
                    "Validation error occurred. Path: {Path}, ValidationErrors: {@ValidationErrors}", 
                    context.Request.Path,
                    validationEx.Errors);
                break;
            case IDMismatchException:
            case InvalidLoginException:
                statusCode = HttpStatusCode.BadRequest;
                logLevel = LogLevel.Warning;
                break;
            case NotFoundException:
            case KeyNotFoundException:
                statusCode = HttpStatusCode.NotFound;
                logLevel = LogLevel.Warning;
                break;
            case UnauthorizedAccessException:
                statusCode = HttpStatusCode.Unauthorized;
                logLevel = LogLevel.Warning;
                break;  
            case TimeoutException:
                statusCode = HttpStatusCode.GatewayTimeout;
                apiResult.Message = "Request timed out";
                logLevel = LogLevel.Error;
                break;
            case NotImplementedException:
                statusCode = HttpStatusCode.NotImplemented;
                apiResult.Message = "Not implemented";
                logLevel = LogLevel.Warning;
                break;
            case FormatException:
                statusCode = HttpStatusCode.BadRequest;
                apiResult.Message = "Invalid format";
                logLevel = LogLevel.Warning;
                break;
            case OperationCanceledException:
                statusCode = HttpStatusCode.RequestTimeout;
                apiResult.Message = "Request was cancelled";
                logLevel = LogLevel.Information;
                break;
            case var ex when exception.GetType().Name == "DbUpdateException":
                statusCode = HttpStatusCode.Conflict;
                apiResult.Message = "Database update error";
                logLevel = LogLevel.Error;
                break;
            default:
                apiResult.Message = "Internal Server Error";
                statusCode = HttpStatusCode.InternalServerError;
                logLevel = LogLevel.Error;
                break;
        }

        context.Response.StatusCode = (int)statusCode;
        
        if (logLevel == LogLevel.Warning || exception is ValidationException)
        {
            _logger.LogWarning(exception,
                "Exception handled: {ExceptionType} | StatusCode: {StatusCode} | Path: {Path} | Method: {Method} | RemoteIP: {RemoteIP} | Message: {Message}",
                exception.GetType().Name,
                (int)statusCode,
                context.Request.Path,
                context.Request.Method,
                context.Connection.RemoteIpAddress,
                exception.Message);
        }
        else
        {
            _logger.LogError(exception,
                "Unhandled exception: {ExceptionType} | StatusCode: {StatusCode} | Path: {Path} | Method: {Method} | RemoteIP: {RemoteIP} | Message: {Message} | StackTrace: {StackTrace}",
                exception.GetType().Name,
                (int)statusCode,
                context.Request.Path,
                context.Request.Method,
                context.Connection.RemoteIpAddress,
                exception.Message,
                exception.StackTrace);
        }

        var settings = new JsonSerializerSettings
        {
            ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver()
        };
        var exceptionResult = JsonConvert.SerializeObject(apiResult, settings);
        return context.Response.WriteAsync(exceptionResult);
    }
}
