using System.Net;
using System.Text.Json;
using HomeInventory.Domain.Common;
using FluentValidation;

namespace HomeInventory.WebApi.Middleware;

public class GlobalExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;

    public GlobalExceptionHandlerMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlerMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var statusCode = HttpStatusCode.InternalServerError;
        var message = "An unexpected error occurred. Please try again later.";
        var errors = new List<string>();

        switch (exception)
        {
            case DomainException domainEx:
                statusCode = HttpStatusCode.BadRequest;
                message = domainEx.Message;
                break;

            case ValidationException validationEx:
                statusCode = HttpStatusCode.BadRequest;
                message = "Validation failed";
                errors = validationEx.Errors.Select(e => e.ErrorMessage).ToList();
                break;

            case KeyNotFoundException:
                statusCode = HttpStatusCode.NotFound;
                message = exception.Message;
                break;

            case UnauthorizedAccessException:
                statusCode = HttpStatusCode.Unauthorized;
                message = "Unauthorized access";
                break;

            default:
                break;
        }

        var response = new
        {
            status = (int)statusCode,
            message,
            errors = errors.Any() ? errors : null
        };

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        var jsonResponse = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        return context.Response.WriteAsync(jsonResponse);
    }
}
