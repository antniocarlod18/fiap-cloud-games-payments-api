using FiapCloudGamesPayments.Application.Dtos;
using FiapCloudGamesPayments.Application.Helpers;
using FiapCloudGamesPayments.Application.Services.Interfaces;
using FiapCloudGamesPayments.Domain.Entities;
using FiapCloudGamesPayments.Domain.Enums;
using FiapCloudGamesPayments.Domain.Exceptions;
using FiapCloudGamesPayments.Domain.Repositories;
using MassTransit;
using MassTransit.Transports;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FiapCloudGamesPayments.Application.Services;

public class OrderPaymentService : IOrderPaymentService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<OrderPaymentService> _logger;

    public OrderPaymentService(IUnitOfWork unitOfWork, ILogger<OrderPaymentService> logger)
    {
        this._unitOfWork = unitOfWork;
        this._logger = logger;
    }

    public async Task ProcessPayment(Guid orderId, Guid userId, decimal price)
    {
        _logger.LogInformation("Starting payment processing for OrderId: {OrderId}, UserId: {UserId}, Price: {Price}", orderId, userId, price);

        var orderPayment = await _unitOfWork.OrderPaymentsRepo.GetByOrderAsync(orderId);

        if (orderPayment != null)
        {
            _logger.LogInformation("Payment already exists for OrderId: {OrderId}. Aborting processing.", orderId);
            throw new ResourceAlreadyExistsException(nameof(OrderPayment));
        }
        var newOrderPayment = new OrderPayment(orderId, userId, PaymentStatusEnum.Processing, price, "BRL", PaymentHelper.GetRandomPaymentMethod());

        await _unitOfWork.OrderPaymentsRepo.AddAsync(newOrderPayment);
        await _unitOfWork.Commit();
    }

    public async Task<OrderPaymentResponseDto?> GetAsync(Guid orderId, Guid idUser, string role)
    {
        _logger.LogInformation("Getting Payment by id {OrderId}", orderId);
        var orderPayment = await _unitOfWork.OrderPaymentsRepo.GetByOrderAsync(orderId);

        if (orderPayment == null || (orderPayment.UserId != idUser && role != "Admin"))
        {
            throw new ResourceNotFoundException(nameof(OrderPayment));
        }        

        _logger.LogInformation("Payment with OrderId {orderId} retrieved", orderId);
        return orderPayment;
    }
}
