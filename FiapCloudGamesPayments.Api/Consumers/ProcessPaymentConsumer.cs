using FiapCloudGames.Contracts.IntegrationEvents;
using FiapCloudGamesPayments.Application.Services.Interfaces;
using FiapCloudGamesPayments.Domain.Exceptions;
using MassTransit;

namespace FiapCloudGamesPayments.Api.Consumers
{
    public class ProcessPaymentConsumer : IConsumer<OrderPlacedIntegrationEvent>
    {
        private readonly IOrderPaymentService _orderPaymentService;
        private readonly ILogger<ProcessPaymentConsumer> _logger;

        public ProcessPaymentConsumer(IOrderPaymentService orderPaymentService, ILogger<ProcessPaymentConsumer> logger)
        {
            this._orderPaymentService = orderPaymentService;
            this._logger = logger;
        }

        public async Task Consume(ConsumeContext<OrderPlacedIntegrationEvent> context)
        {
            var msg = context.Message;
            _logger.LogInformation("Received OrderPlaced event. OrderId: {OrderId}, UserId: {UserId}, Price: {Price}", msg.OrderId, msg.UserId, msg.Price);

            try
            {
                await _orderPaymentService.ProcessPayment(msg.OrderId, msg.UserId, msg.Price);
            }
            catch (ResourceAlreadyExistsException)
            {
                _logger.LogInformation("Idempotency: payment already exists for OrderId: {OrderId}. Acknowledging message.", msg.OrderId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing payment for OrderId: {OrderId}", msg.OrderId);
                throw; 
            }
        }
    }
}
