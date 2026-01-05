using FluentValidation;

namespace TaskFlow.API.Middleware;

public sealed class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
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
        catch (Exception exception)
        {
            _logger.LogError(exception, "Unhandled exception for request {Method} {Path}", context.Request.Method, context.Request.Path);

            var (statusCode, title) = exception switch
            {
                ValidationException => (StatusCodes.Status400BadRequest, "Validation failed"),
                ArgumentException => (StatusCodes.Status400BadRequest, "Invalid request"),
                KeyNotFoundException => (StatusCodes.Status404NotFound, "Resource not found"),
                InvalidOperationException => (StatusCodes.Status409Conflict, "Operation conflict"),
                UnauthorizedAccessException => (StatusCodes.Status401Unauthorized, "Unauthorized"),
                _ => (StatusCodes.Status500InternalServerError, "Server error")
            };

            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/json";

            var payload = new
            {
                title,
                status = statusCode,
                detail = exception.Message
            };

            await context.Response.WriteAsJsonAsync(payload);
        }
    }
}
