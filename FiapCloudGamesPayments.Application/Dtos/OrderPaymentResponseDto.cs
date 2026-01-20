using FiapCloudGamesPayments.Domain.Entities;
using FiapCloudGamesPayments.Domain.Enums;

namespace FiapCloudGamesPayments.Application.Dtos;

public class OrderPaymentResponseDto
{
    public Guid Id { get; set; }
    public Guid OrderId { get; set; }
    public Guid UserId { get; set; }
    public string Status { get; set; }
    public decimal Price { get; set; }
    public DateTime DateCreated { get; set; }
    public DateTime? DateUpdated { get; set; }

    public static implicit operator OrderPaymentResponseDto?(OrderPayment? orderPayment)
    {
        if (orderPayment == null) return null;

        return new OrderPaymentResponseDto
        {
            Id = orderPayment.Id,
            OrderId = orderPayment.OrderId,
            UserId = orderPayment.UserId,
            Status = orderPayment.Status.ToString(),
            Price = orderPayment.Price,
            DateCreated = orderPayment.DateCreated,
            DateUpdated = orderPayment.DateUpdated
        };
    }
}