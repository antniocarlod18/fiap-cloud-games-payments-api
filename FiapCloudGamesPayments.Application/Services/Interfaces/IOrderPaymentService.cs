using FiapCloudGamesPayments.Application.Dtos;

namespace FiapCloudGamesPayments.Application.Services.Interfaces;

public interface IOrderPaymentService
{
    Task ProcessPayment(Guid orderId, Guid userId, decimal price);
    Task<OrderPaymentResponseDto?> GetAsync(Guid orderId, Guid idUser, string role);
}
