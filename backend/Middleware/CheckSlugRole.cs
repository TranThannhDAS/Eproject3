using backend.Entity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using webapi.Data;

namespace webapi.Middleware
{
    public class CheckSlugRole
    {
        private readonly RequestDelegate _next;
        private readonly TokenValidationParameters _validationParameters;
        private readonly IConfiguration configuration;

        public CheckSlugRole(
            RequestDelegate __next,
            IOptions<JwtBearerOptions> jwtOptions,
            IConfiguration configuration
        )
        {
            _next = __next ?? throw new ArgumentNullException("Not Found Next Delegate");
            _validationParameters = jwtOptions.Value.TokenValidationParameters;
            this.configuration = configuration;
        }

        public async Task InvokeAsync(HttpContext context, IServiceProvider serviceProvider)
        {
            if (context == null)
            {
                throw new ArgumentNullException("Context not found");
            }

            // Check nếu login thì pass middlware
            if (context.Request.Path.Equals("/api/Users/Login", StringComparison.OrdinalIgnoreCase))
            {
                await _next(context);
                return;
            }

            // Check nếu remove confirm code thì pass middlware
            if (
                context.Request.Path
                    .ToString()
                    .Contains("/api/Users/DeleteConfirmCode", StringComparison.OrdinalIgnoreCase)
            )
            {
                await _next(context);
                return;
            }

            // Check nếu forgotpassword thì pass middlware
            if (
                context.Request.Path.Equals(
                    "/api/Users/SendForgotPasswordEmail",
                    StringComparison.OrdinalIgnoreCase
                )
            )
            {
                await _next(context);
                return;
            }

            // Check nếu verify code thì pass middlware
            if (
                context.Request.Path.Equals(
                    "/api/Users/VerifyCodeResetPassword",
                    StringComparison.OrdinalIgnoreCase
                )
            )
            {
                await _next(context);
                return;
            }

            // Check nếu logout thì pass middlware
            if (
                context.Request.Path.Equals("/api/Users/Logout", StringComparison.OrdinalIgnoreCase)
            )
            {
                await _next(context);
                return;
            }

            // Check nếu đăng ký thì pass middlware
            if (
                context.Request.Path.Equals(
                    "/api/Users/Register",
                    StringComparison.OrdinalIgnoreCase
                )
            )
            {
                await _next(context);
                return;
            }

            // Check neu refresh token thi pass
            if (
                context.Request.Path.Equals(
                    "/api/Users/Refresh",
                    StringComparison.OrdinalIgnoreCase
                )
            )
            {
                await _next(context);
                return;
            }

            string token = context.Request.Headers["Authorization"]
                .FirstOrDefault()
                .Split(" ")
                .Last();

            string requestUri = context.Request.GetEncodedUrl();

            string slug = requestUri;
            Slug slugRole = null;

            if (string.IsNullOrEmpty(token))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Missing JWT token.");
                return;
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(configuration["JWT:SecretKey"]);

            var _db = serviceProvider.GetRequiredService<DataContext>();

            try
            {
                // Check slug
                var jwtToken = tokenHandler.ReadJwtToken(token);
                var users = jwtToken.Claims
                    .Where(r => r.Type == JwtRegisteredClaimNames.UniqueName)
                    .Select(r => r.Value)
                    .ToList();

                foreach (var user in users)
                {
                    var us = _db.Users.FirstOrDefault(u => u.Username.Equals(user));
                    int ro = int.Parse(us.RoleId.ToString());

                    //slugRole = _db.Slugs.FirstOrDefault(
                    //    s => s.URI.Trim() == slug && s.RoleId == ro
                    //);
                    slugRole = _db.Slugs.FirstOrDefault(
                        s => slug.Contains(s.URI.Trim()) && s.RoleId == ro
                    );
                }

                if (slugRole == null)
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsJsonAsync(
                        new { Message = "You don't have permission to access this endpoint." }
                    );
                    return;
                }
            }
            catch (SecurityTokenException)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsJsonAsync(new { Message = "Invalid JWT token." });
            }

            await _next(context);
        }
    }
}
