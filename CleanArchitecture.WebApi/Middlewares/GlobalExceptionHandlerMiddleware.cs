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

        switch (exception)
        {
            case ArgumentException:
                apiResult.Message = exception.Message;
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                break;
            case ValidationException:
                apiResult.Message = exception.Message;
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
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
