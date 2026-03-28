using Microsoft.Extensions.Configuration;

namespace UserManagementAPI.Middleware;

public class TokenAuthenticationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<TokenAuthenticationMiddleware> _logger;
    private readonly string? _expectedToken;

    public TokenAuthenticationMiddleware(
        RequestDelegate next,
        ILogger<TokenAuthenticationMiddleware> logger,
        IConfiguration configuration)
    {
        _next = next;
        _logger = logger;
        _expectedToken = configuration["Auth:Token"] ?? configuration["USERMANAGEMENTAPI_TOKEN"];
    }

    public async Task Invoke(HttpContext context)
    {
        // Only secure API endpoints (allow OpenAPI documents).
        var path = context.Request.Path.Value ?? string.Empty;
        var isApi = path.StartsWith("/api/", StringComparison.OrdinalIgnoreCase) || path.Equals("/api", StringComparison.OrdinalIgnoreCase);
        var isOpenApi = path.StartsWith("/openapi", StringComparison.OrdinalIgnoreCase);

        if (!isApi || isOpenApi)
        {
            await _next(context);
            return;
        }

        if (string.IsNullOrWhiteSpace(_expectedToken))
        {
            // If the token isn't configured, fail closed.
            await WriteUnauthorizedAsync(context);
            _logger.LogWarning("HTTP {Method} {Path} responded 401. Token not configured.",
                context.Request.Method,
                context.Request.Path);
            return;
        }

        var authHeader = context.Request.Headers.Authorization.ToString();
        if (string.IsNullOrWhiteSpace(authHeader) || !authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        {
            await WriteUnauthorizedAsync(context);
            _logger.LogWarning("HTTP {Method} {Path} responded 401. Missing/invalid Authorization header.",
                context.Request.Method,
                context.Request.Path);
            return;
        }

        var token = authHeader["Bearer ".Length..].Trim();
        if (!string.Equals(token, _expectedToken, StringComparison.Ordinal))
        {
            await WriteUnauthorizedAsync(context);
            _logger.LogWarning("HTTP {Method} {Path} responded 401. Invalid token.",
                context.Request.Method,
                context.Request.Path);
            return;
        }

        await _next(context);
    }

    private static Task WriteUnauthorizedAsync(HttpContext context)
    {
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        context.Response.ContentType = "application/json";

        var payload = new { error = "Unauthorized." };
        return context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(payload));
    }
}

