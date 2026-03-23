using FiapCloudGamesPayments.Domain.Entities;
using FiapCloudGamesPayments.Domain.Enums;
using MediatR;
using System.Text.Json.Serialization;

namespace FiapCloudGamesPayments.Domain.Events
{
    public class OrderPaymentCreatedDomainEvent : IDomainEvent, INotification
    {
        public DateTime OccurredOn { get; private set; }
        public Guid OrderId { get; private set; }
        public Guid UserId { get; private set; }
        public decimal Price { get; set; }
        public string Currency { get; set; } = "BRL";
        public string Method { get; set; } = string.Empty;
        public OrderPaymentCreatedDomainEvent(Guid orderId, Guid userId, decimal price, string currency, string method)
        {
            OrderId = orderId;
            UserId = userId;
            Price = price;
            Currency = currency;
            Method = method;
            OccurredOn = DateTime.UtcNow;
        }    
    }
}
