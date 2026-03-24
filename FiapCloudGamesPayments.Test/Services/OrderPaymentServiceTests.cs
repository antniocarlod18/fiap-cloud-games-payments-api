using System;
using System.Threading.Tasks;
using FiapCloudGamesPayments.Application.Services;
using FiapCloudGamesPayments.Domain.Entities;
using FiapCloudGamesPayments.Domain.Enums;
using FiapCloudGamesPayments.Domain.Exceptions;
using FiapCloudGamesPayments.Domain.Repositories;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace FiapCloudGamesPayments.Test.Services
{
    public class OrderPaymentServiceTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IOrderPaymentRepository> _orderPaymentRepoMock;
        private readonly Mock<ILogger<OrderPaymentService>> _loggerMock;

        public OrderPaymentServiceTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _orderPaymentRepoMock = new Mock<IOrderPaymentRepository>();
            _loggerMock = new Mock<ILogger<OrderPaymentService>>();

            _unitOfWorkMock.Setup(u => u.OrderPaymentsRepo).Returns(_orderPaymentRepoMock.Object);
        }

        [Fact]
        public async Task ProcessPayment_WhenOrderDoesNotExist_AddsAndCommits()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            decimal price = 50m;

            _orderPaymentRepoMock.Setup(r => r.GetByOrderAsync(orderId)).ReturnsAsync((OrderPayment?)null);
            _orderPaymentRepoMock.Setup(r => r.AddAsync(It.IsAny<OrderPayment>())).Returns(Task.CompletedTask).Verifiable();
            _unitOfWorkMock.Setup(u => u.Commit()).Returns(Task.CompletedTask).Verifiable();

            var service = new OrderPaymentService(_unitOfWorkMock.Object, _loggerMock.Object);

            // Act
            await service.ProcessPayment(orderId, userId, price);

            // Assert
            _orderPaymentRepoMock.Verify(r => r.AddAsync(It.Is<OrderPayment>(op => op.OrderId == orderId && op.UserId == userId && op.Price == price)), Times.Once);
            _unitOfWorkMock.Verify(u => u.Commit(), Times.Once);
        }

        [Fact]
        public async Task ProcessPayment_WhenOrderAlreadyExists_ThrowsResourceAlreadyExistsException()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            var existing = new OrderPayment(orderId, userId, PaymentStatusEnum.Approved, 10m, "BRL", "CARD");
            _orderPaymentRepoMock.Setup(r => r.GetByOrderAsync(orderId)).ReturnsAsync(existing);

            var service = new OrderPaymentService(_unitOfWorkMock.Object, _loggerMock.Object);

            // Act & Assert
            await Assert.ThrowsAsync<ResourceAlreadyExistsException>(() => service.ProcessPayment(orderId, userId, 100m));

            _orderPaymentRepoMock.Verify(r => r.AddAsync(It.IsAny<OrderPayment>()), Times.Never);
            _unitOfWorkMock.Verify(u => u.Commit(), Times.Never);
        }

        [Fact]
        public async Task GetAsync_WhenOrderExistsAndUserMatches_ReturnsDto()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            var orderPayment = new OrderPayment(orderId, userId, PaymentStatusEnum.Approved, 20m, "BRL", "CARD");
            _orderPaymentRepoMock.Setup(r => r.GetByOrderAsync(orderId)).ReturnsAsync(orderPayment);

            var service = new OrderPaymentService(_unitOfWorkMock.Object, _loggerMock.Object);

            // Act
            var result = await service.GetAsync(orderId, userId, "User");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(orderId, result.OrderId);
            Assert.Equal(userId, result.UserId);
        }

        [Fact]
        public async Task GetAsync_WhenOrderNotFound_ThrowsResourceNotFoundException()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            _orderPaymentRepoMock.Setup(r => r.GetByOrderAsync(orderId)).ReturnsAsync((OrderPayment?)null);

            var service = new OrderPaymentService(_unitOfWorkMock.Object, _loggerMock.Object);

            // Act & Assert
            await Assert.ThrowsAsync<ResourceNotFoundException>(() => service.GetAsync(orderId, userId, "User"));
        }

        [Fact]
        public async Task GetAsync_WhenUserMismatchAndNotAdmin_ThrowsResourceNotFoundException()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var ownerId = Guid.NewGuid();
            var callerId = Guid.NewGuid();

            var orderPayment = new OrderPayment(orderId, ownerId, PaymentStatusEnum.Approved, 20m, "BRL", "CARD");
            _orderPaymentRepoMock.Setup(r => r.GetByOrderAsync(orderId)).ReturnsAsync(orderPayment);

            var service = new OrderPaymentService(_unitOfWorkMock.Object, _loggerMock.Object);

            // Act & Assert
            await Assert.ThrowsAsync<ResourceNotFoundException>(() => service.GetAsync(orderId, callerId, "User"));
        }

        [Fact]
        public async Task GetAsync_WhenUserMismatchButAdmin_ReturnsDto()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var ownerId = Guid.NewGuid();
            var callerId = Guid.NewGuid();

            var orderPayment = new OrderPayment(orderId, ownerId, PaymentStatusEnum.Approved, 20m, "BRL", "CARD");
            _orderPaymentRepoMock.Setup(r => r.GetByOrderAsync(orderId)).ReturnsAsync(orderPayment);

            var service = new OrderPaymentService(_unitOfWorkMock.Object, _loggerMock.Object);

            // Act
            var result = await service.GetAsync(orderId, callerId, "Admin");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(orderId, result.OrderId);
        }
    }
}
