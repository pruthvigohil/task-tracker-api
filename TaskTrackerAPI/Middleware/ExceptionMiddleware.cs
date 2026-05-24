using System.Net;
using System.Text.Json;
using TaskTrackerAPI.Exceptions;

namespace TaskTrackerAPI.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
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
            _logger.LogError(ex, "Unhandled exception: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        var (statusCode, message) = ex switch
        {
            NotFoundException _ => (HttpStatusCode.NotFound, ex.Message),
            UnauthorizedException _ => (HttpStatusCode.Unauthorized, ex.Message),
            ConflictException _ => (HttpStatusCode.Conflict, ex.Message),
            ForbiddenException _ => (HttpStatusCode.Forbidden, ex.Message),
            _ => (HttpStatusCode.InternalServerError, "An unexpected error occurred.")
        };

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        var result = JsonSerializer.Serialize(new { message });
        return context.Response.WriteAsync(result);
    }
}