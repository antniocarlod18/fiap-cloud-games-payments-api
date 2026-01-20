namespace FiapCloudGamesPayments.Domain.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        IOrderPaymentRepository OrderPaymentsRepo { get; }
        Task Commit();
    }
}
