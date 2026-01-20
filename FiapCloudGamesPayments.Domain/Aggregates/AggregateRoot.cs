using FiapCloudGamesPayments.Domain.Entities;
using FiapCloudGamesPayments.Domain.Events;
using System.Diagnostics.CodeAnalysis;

namespace FiapCloudGamesPayments.Domain.Aggregates
{
    public abstract class AggregateRoot : EntityBase
    {
        private readonly List<IDomainEvent> _domainEvents = [];

        [SetsRequiredMembers]
        protected AggregateRoot() : base()
        {
        }

        public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents;

        protected void AddDomainEvent(IDomainEvent domainEvent)
            => _domainEvents.Add(domainEvent);

        public void ClearDomainEvents()
            => _domainEvents.Clear();
    }
}
