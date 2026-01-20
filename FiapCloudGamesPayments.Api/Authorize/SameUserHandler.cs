using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Text.Json;

namespace FiapCloudGamesPayments.Api.Authorize
{
    public class SameUserHandler : AuthorizationHandler<SameUserRequirement>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SameUserHandler(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            SameUserRequirement requirement)
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null)
                return;

            var userRole = context.User.FindFirst(ClaimTypes.Role)?.Value;
            var userIdFromToken = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userRole == "Admin")
            {
                context.Succeed(requirement);
                return;
            }

            if ((userRole == "User") && userIdFromToken != null)
            {
                string? userIdFromRequest = null;

                if (httpContext.Request.RouteValues.TryGetValue("userId", out var routeValue))
                    userIdFromRequest = routeValue?.ToString();

                else if (httpContext.Request.ContentLength > 0 &&
                         httpContext.Request.ContentType?.Contains("application/json") == true)
                {
                    httpContext.Request.EnableBuffering();
                    using var reader = new StreamReader(httpContext.Request.Body, leaveOpen: true);
                    var body = await reader.ReadToEndAsync();
                    httpContext.Request.Body.Position = 0;

                    using var doc = JsonDocument.Parse(body);
                    if (doc.RootElement.TryGetProperty("userId", out var idProp))
                        userIdFromRequest = idProp.GetString();
                }

                if (userIdFromRequest == userIdFromToken)
                {
                    context.Succeed(requirement);
                    return;
                }
            }

            context.Fail();
        }
    }
}
