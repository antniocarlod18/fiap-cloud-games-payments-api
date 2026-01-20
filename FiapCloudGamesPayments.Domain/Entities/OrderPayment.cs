using FiapCloudGamesPayments.Domain.Aggregates;
using FiapCloudGamesPayments.Domain.Enums;
using FiapCloudGamesPayments.Domain.Events;
using System.Diagnostics.CodeAnalysis;

namespace FiapCloudGamesPayments.Domain.Entities;

public class OrderPayment : AggregateRoot
{
    public Guid OrderId { get; set; }
    public Guid UserId { get; set; }
    public PaymentStatusEnum Status { get; set; }
    public decimal Price { get;  set; }

    [SetsRequiredMembers]
    public OrderPayment(Guid orderId, Guid userId, PaymentStatusEnum status, decimal price) : base()
    {
        OrderId = orderId;
        UserId = userId;
        Status = status;
        Price = price;
        AddDomainEvent(new OrderPaymentCreatedDomainEvent(OrderId, UserId, Status));
    }
}
