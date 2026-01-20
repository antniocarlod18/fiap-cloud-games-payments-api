using FiapCloudGamesPayments.Domain.Abstractions;
using FiapCloudGamesPayments.Domain.Aggregates;
using FiapCloudGamesPayments.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FiapCloudGamesPayments.Infra.Data.Context
{
    public class ContextDb : DbContext
    {
        private readonly IDomainEventDispatcher _dispatcher;
        public ContextDb(
            DbContextOptions<ContextDb> options,
            IDomainEventDispatcher dispatcher)
            : base(options)
        {
            _dispatcher = dispatcher;
        }

        public DbSet<OrderPayment> OrderPayments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ContextDb).Assembly);
        }

        public override async Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default)
        {
            var domainEvents = ChangeTracker
                .Entries<AggregateRoot>()
                .SelectMany(e => e.Entity.DomainEvents)
                .ToList();

            var result = await base.SaveChangesAsync(cancellationToken);

            await _dispatcher.DispatchAsync(domainEvents);

            foreach (var entry in ChangeTracker.Entries<AggregateRoot>())
                entry.Entity.ClearDomainEvents();

            return result;
        }
    }
}
