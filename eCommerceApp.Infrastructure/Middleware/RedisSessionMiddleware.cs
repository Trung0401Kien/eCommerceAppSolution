using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace eCommerceApp.Infrastructure.Middleware
{
    public class RedisSessionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConnectionMultiplexer _redis;
        private readonly IConfiguration _configuration;

        public RedisSessionMiddleware(RequestDelegate next, IConnectionMultiplexer redis, IConfiguration configuration)
        {
            _next = next;
            _redis = redis;
            _configuration = configuration;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.Path.StartsWithSegments("/api/authentication/login", StringComparison.OrdinalIgnoreCase))
            {
                await _next(context);
                return;
            }
            var token = context.Request.Cookies["Access_Token"];

            if (string.IsNullOrEmpty(token))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Token is missing");
                return;
            }

            var jwtHandler = new JwtSecurityTokenHandler();
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Key"])),
                ValidIssuer = _configuration["JWT:Issuer"],
                ValidAudience = _configuration["JWT:Audience"],
                ClockSkew = TimeSpan.Zero
            };

            try
            {
                var principal = jwtHandler.ValidateToken(token, tokenValidationParameters, out var validatedToken);

                var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var sessionId = principal.FindFirst("sessionId")?.Value;
                if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(sessionId))
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsync("User or session not authenticated");
                    return;
                }
                var db = _redis.GetDatabase();
                var sessionKey = $"session:{userId}:{sessionId}";
                var currentSession = await db.StringGetAsync(sessionKey);

                if (currentSession.IsNullOrEmpty || currentSession != "true")
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsync("Session is not valid or has been logged out");
                    return;
                }

                await _next(context);
            }
            catch (SecurityTokenExpiredException)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Token has expired");
            }
            catch (Exception ex)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync($"Invalid token: {ex.Message}");
            }
        }

    }
}