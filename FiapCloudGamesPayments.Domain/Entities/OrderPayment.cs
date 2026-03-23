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
    public string Currency { get; set; } = "BRL";
    public string Method { get; set; } = string.Empty;

    [SetsRequiredMembers]
    public OrderPayment(Guid orderId, Guid userId, PaymentStatusEnum status, decimal price, string currency, string method) : base()
    {
        OrderId = orderId;
        UserId = userId;
        Status = status;
        Price = price;
        Currency = currency;
        Method = method;
        AddDomainEvent(new OrderPaymentCreatedDomainEvent(OrderId, userId, price, Currency, Method));
    }
}
