using Azure.Messaging.ServiceBus;
using FiapCloudGames.Contracts.IntegrationEvents;
using FiapCloudGamesPayments.Domain.Events;
using MassTransit;
using MassTransit.Middleware;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FiapCloudGamesPayments.Application.EventHandler
{
    public class OrderPaymentCreatedEventHandler : INotificationHandler<OrderPaymentCreatedDomainEvent>
    {
        private readonly ServiceBusSender _sender;
        private readonly ILogger<OrderPaymentCreatedEventHandler> _logger;

        public OrderPaymentCreatedEventHandler(IConfiguration configuration, ILogger<OrderPaymentCreatedEventHandler> logger)
        {
            var client = new ServiceBusClient(configuration["ServiceBus:ConnectionString"]);
            this._sender = client.CreateSender(configuration["ServiceBus:QueueName"]);
            this._logger = logger;
        }

        public async Task Handle(OrderPaymentCreatedDomainEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling OrderPaymentCreatedDomainEvent. OrderId: {OrderId}, UserId: {UserId}", 
                notification.OrderId, notification.UserId);

            try
            {
                var payment = new PaymentMessage
                {
                    OrderId = notification.OrderId.ToString(),
                    UserId = notification.UserId.ToString(),
                    Price = notification.Price,
                    Currency = notification.Currency,
                    Method = notification.Method
                };

                var json = JsonSerializer.Serialize(payment);

                var message = new ServiceBusMessage(json);

                await _sender.SendMessageAsync(message);

                _logger.LogInformation("Sent PaymentMessage to Azure Service Bus queue 'payment-queue' for OrderId: {OrderId}", notification.OrderId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error publishing PaymentProcessedIntegrationEvent for OrderId: {OrderId}", notification.OrderId);
                throw;
            }

            return;
        }
    }

    public class PaymentMessage
    {
        [JsonPropertyName("orderId")]
        public string OrderId { get; set; } = string.Empty;

        [JsonPropertyName("userId")]
        public string UserId { get; set; } = string.Empty;

        [JsonPropertyName("price")]
        public decimal Price { get; set; }

        [JsonPropertyName("currency")]
        public string Currency { get; set; } = "BRL";

        [JsonPropertyName("method")]
        public string Method { get; set; } = string.Empty;
    }
}
