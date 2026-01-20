using FiapCloudGamesPayments.Domain.Enums;
using FiapCloudGamesPayments.Domain.Entities;
using MediatR;

namespace FiapCloudGamesPayments.Domain.Events
{
    public class OrderPaymentCreatedDomainEvent : IDomainEvent, INotification
    {
        public DateTime OccurredOn { get; private set; }
        public Guid OrderId { get; private set; }
        public Guid UserId { get; private set; }
        public PaymentStatusEnum Status { get; set; }
        public OrderPaymentCreatedDomainEvent(Guid orderId, Guid userId, PaymentStatusEnum status)
        {
            OrderId = orderId;
            UserId = userId;
            Status = status;
            OccurredOn = DateTime.UtcNow;
        }    
    }
}
