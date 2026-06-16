using SchoolERP.Repositories.Interfaces;

namespace SchoolERP.Middleware
{
    public class TokenBlacklistMiddleware
    {
        private readonly RequestDelegate _next;

        public TokenBlacklistMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context,
            ITokenBlacklistRepository blacklistRepo)
        {
            if (context.Request.Path.Value?.ToLower().Contains("/api/login/login") == true)
            {
                await _next(context);
                return;
            }
            string? authHeader = context.Request.Headers["Authorization"];

            if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer "))
            {
                string token = authHeader.Substring("Bearer ".Length).Trim();

                bool isBlacklisted = await blacklistRepo.IsBlacklistedAsync(token);

                if (isBlacklisted)
                {
                    context.Response.StatusCode = 401;
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsJsonAsync(new
                    {
                        statusCode = 401,
                        error = "Token has been invalidated. Please login again."
                    });
                    return;
                }
            }

            await _next(context); 
        }
    }
}