using FiapCloudGamesPayments.Domain.Events;

namespace FiapCloudGamesPayments.Domain.Abstractions;

public interface IDomainEventDispatcher
{
    Task DispatchAsync(IEnumerable<IDomainEvent> events);
}
