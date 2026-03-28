namespace UserManagementAPI.Middleware;

public class RequestResponseLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestResponseLoggingMiddleware> _logger;

    public RequestResponseLoggingMiddleware(RequestDelegate next, ILogger<RequestResponseLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        var method = context.Request.Method;
        var path = context.Request.Path.Value ?? context.Request.Path.ToString();

        await _next(context);

        _logger.LogInformation("HTTP {Method} {Path} responded {StatusCode}",
            method,
            path,
            context.Response.StatusCode);
    }
}

