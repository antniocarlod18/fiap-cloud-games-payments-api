using System;
using System.Collections.Generic;
using System.Text;

namespace FiapCloudGames.Contracts.IntegrationEvents;

public record OrderPlacedIntegrationEvent
{
    public Guid UserId { get; init; }
    public Guid OrderId { get; init; }
    public decimal Price { get; init; }
    public IList<Guid> GameIds { get; init; }
}
