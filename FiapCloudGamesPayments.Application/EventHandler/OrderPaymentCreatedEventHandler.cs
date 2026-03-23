using FiapCloudGames.Contracts.IntegrationEvents;
using FiapCloudGamesPayments.Domain.Events;
using MassTransit;
using MassTransit.Middleware;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FiapCloudGamesPayments.Application.EventHandler
{
    public class OrderPaymentCreatedEventHandler : INotificationHandler<OrderPaymentCreatedDomainEvent>
    {
        private readonly ISendEndpointProvider _sendEndpointProvider;
        private readonly ILogger<OrderPaymentCreatedEventHandler> _logger;

        public OrderPaymentCreatedEventHandler(ISendEndpointProvider sendEndpointProvider, ILogger<OrderPaymentCreatedEventHandler> logger)
        {
            this._sendEndpointProvider = sendEndpointProvider;
            this._logger = logger;
        }

        public async Task Handle(OrderPaymentCreatedDomainEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling OrderPaymentCreatedDomainEvent. OrderId: {OrderId}, UserId: {UserId}", 
                notification.OrderId, notification.UserId);

            try
            {
                var endpoint = await _sendEndpointProvider.GetSendEndpoint(
                    new Uri("queue:payment-queue")
                );

                await endpoint.Send(new
                {
                    notification.OrderId,
                    notification.UserId,
                    notification.Price,
                    notification.Currency,
                    notification.Method
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
