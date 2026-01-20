using FiapCloudGamesPayments.Domain.Abstractions;
using FiapCloudGamesPayments.Domain.Events;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace FiapCloudGamesPayments.Infra.Data.Messaging
{
    public class MediatRDomainEventDispatcher : IDomainEventDispatcher
    {
        private readonly IMediator _mediator;

        public MediatRDomainEventDispatcher(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task DispatchAsync(IEnumerable<IDomainEvent> events)
        {
            foreach (var domainEvent in events)
            {
                await _mediator.Publish(domainEvent);
            }
        }
    }
}
