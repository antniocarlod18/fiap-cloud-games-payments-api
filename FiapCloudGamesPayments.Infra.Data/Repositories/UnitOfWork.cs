using FiapCloudGamesPayments.Domain.Repositories;
using FiapCloudGamesPayments.Infra.Data.Context;

namespace FiapCloudGamesPayments.Infra.Data.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly ContextDb _context;
    private IOrderPaymentRepository _orderPaymentRepository;

    public UnitOfWork(ContextDb contextDb)
    {
        _context = contextDb;
    }

    public IOrderPaymentRepository OrderPaymentsRepo
    {
        get
        {
            if (_orderPaymentRepository == null)
            {
                _orderPaymentRepository = new OrderPaymentRepository(_context);
            }
            return _orderPaymentRepository;
        }
    }

    public async Task Commit()
    {
        await _context.SaveChangesAsync();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
