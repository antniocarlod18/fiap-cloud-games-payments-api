using FiapCloudGames.Contracts.IntegrationEvents;
using FiapCloudGamesPayments.Domain.Events;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FiapCloudGamesPayments.Application.EventHandler
{
    public class OrderPaymentCreatedEventHandler : INotificationHandler<OrderPaymentCreatedDomainEvent>
    {
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ILogger<OrderPaymentCreatedEventHandler> _logger;

        public OrderPaymentCreatedEventHandler(IPublishEndpoint publishEndpoint, ILogger<OrderPaymentCreatedEventHandler> logger)
        {
            this._publishEndpoint = publishEndpoint;
            this._logger = logger;
        }

        public async Task Handle(OrderPaymentCreatedDomainEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling OrderPaymentCreatedDomainEvent. OrderId: {OrderId}, UserId: {UserId}, Status: {Status}", notification.OrderId, notification.UserId, notification.Status);

            try
            {
                await _publishEndpoint.Publish<PaymentProcessedIntegrationEvent>(new()
                {
                    OrderId = notification.OrderId,
                    UserId = notification.UserId,
                    Status = notification.Status
                });

                _logger.LogInformation("Published PaymentProcessedIntegrationEvent for OrderId: {OrderId}", notification.OrderId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error publishing PaymentProcessedIntegrationEvent for OrderId: {OrderId}", notification.OrderId);
                throw;
            }

            return;
        }
    }
}
