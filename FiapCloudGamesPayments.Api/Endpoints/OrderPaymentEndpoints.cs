using FiapCloudGamesPayments.Application.Services.Interfaces;
using System.Security.Claims;

namespace FiapCloudGamesPayments.Api.Endpoints
{
    public static class OrderPaymentEndpoints
    {
        public static IEndpointRouteBuilder MapOrderPaymentEndpoints(this IEndpointRouteBuilder endpoints)
        {
            endpoints.MapGet("/orders/{orderId}/payments", GetPaymentsByOrderAsync)
                .RequireAuthorization(policy => policy.RequireRole("Admin", "User"));

            return endpoints;
        }

        public static async Task<IResult> GetPaymentsByOrderAsync(Guid orderId, IOrderPaymentService service, HttpContext context)
        {
            Guid.TryParse(context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var userId);
            var role = context.User.FindFirst(ClaimTypes.Role)?.Value;

            var orderPaymentResponse = await service.GetAsync(orderId, userId, role);
            return Results.Ok(orderPaymentResponse);
        }

    }
}
