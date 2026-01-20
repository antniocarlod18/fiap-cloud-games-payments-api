using FiapCloudGamesPayments.Domain.Entities;
using FiapCloudGamesPayments.Domain.Repositories;
using FiapCloudGamesPayments.Infra.Data.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace FiapCloudGamesPayments.Infra.Data.Repositories
{
    public class OrderPaymentRepository : Repository<OrderPayment>, IOrderPaymentRepository
    {
        private readonly ContextDb _context;

        public OrderPaymentRepository(ContextDb contextDb) : base(contextDb)
        {
            this._context = contextDb;
        }

        public async Task<OrderPayment?> GetByOrderAsync(Guid orderId)
        {
            return await _context.OrderPayments
                .AsNoTracking()
                .FirstOrDefaultAsync(op => op.OrderId == orderId);
        }
    }
}
