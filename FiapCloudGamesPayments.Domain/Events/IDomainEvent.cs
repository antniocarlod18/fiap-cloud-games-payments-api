using System;
using System.Collections.Generic;
using System.Text;

namespace FiapCloudGamesPayments.Domain.Events
{
    public interface IDomainEvent
    {
        DateTime OccurredOn { get; }
    }
}
