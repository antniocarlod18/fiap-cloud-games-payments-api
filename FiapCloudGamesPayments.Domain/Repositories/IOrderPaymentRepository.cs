using FiapCloudGamesPayments.Domain.Entities;

namespace FiapCloudGamesPayments.Domain.Repositories;

public interface IOrderPaymentRepository : IRepository<OrderPayment>
{
    Task<OrderPayment?> GetByOrderAsync(Guid orderId);
}
