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
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                break;
            case NotFoundException:
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                break;
            case UnauthorizedAccessException:
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                break;
            default:
                apiResult.Message = "Internal Server Error";
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                break;
        }

        _logger.LogError(exception, "An error has occurred: {Message}", exception.Message);

        var exceptionResult = JsonConvert.SerializeObject(apiResult);
        return context.Response.WriteAsync(exceptionResult);
    }
}
