using Application.Commands.Orders;
using Domain.Aggregates.Basket;
using Domain.Aggregates.Orders;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Exceptions.Customers;
using Domain.Exceptions.Orders;
using Domain.Repositories;
using Moq;
using Domain.ValueObjects;
using Xunit;

namespace Application.Tests.Commands.Orders
{
    public class ConvertBasketToOrderCommandHandlerTests
    {
        private static readonly Guid ValidCustomerId = Guid.NewGuid();
        private static readonly Guid ValidBasketId = Guid.NewGuid();
        private static readonly Guid ValidProductId = Guid.NewGuid();
        private static readonly String ValidProductName = "Test Product";
        private static readonly String ValidProductDescription = "Test Description";
        private static readonly String ValidEmail = "test@example.com";
        private static readonly String ValidName = "John Doe";
        private static readonly Address ValidAddress = new(
            "123 Main Street",
            "Apartment 4B",
            "London",
            "Greater London",
            "SW1A 1AA"
        );
        private static readonly CardDetails ValidCardDetails = new("1234567890123456", "12/25", "123");
        private static readonly Decimal ValidPrice = 10.00m;
        private static readonly Int32 ValidQuantity = 1;

        private readonly Mock<IBasketRepository> _basketRepositoryMock;
        private readonly Mock<IProductRepository> _productRepositoryMock;
        private readonly Mock<IOrderRepository> _orderRepositoryMock;
        private readonly Mock<ICustomerRepository> _customerRepositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly ConvertBasketToOrderCommandHandler _handler;
        private readonly Basket _basket;
        private readonly Customer _customer;
        private readonly Product _product;

        public ConvertBasketToOrderCommandHandlerTests()
        {
            _basketRepositoryMock = new Mock<IBasketRepository>();
            _productRepositoryMock = new Mock<IProductRepository>();
            _orderRepositoryMock = new Mock<IOrderRepository>();
            _customerRepositoryMock = new Mock<ICustomerRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();

            _handler = new ConvertBasketToOrderCommandHandler(
                _basketRepositoryMock.Object,
                _productRepositoryMock.Object,
                _orderRepositoryMock.Object,
                _customerRepositoryMock.Object,
                _unitOfWorkMock.Object);

            _basket = new Basket(ValidBasketId, ValidCustomerId);
            _basket.AddItem(new BasketItem(ValidProductId, ValidQuantity, ValidPrice, ValidProductName));

            _customer = new Customer(ValidCustomerId, ValidEmail, ValidName, ValidAddress, ValidCardDetails);

            _product = new Product(ValidProductId, ValidProductName, ValidProductDescription, false, "https://www.test.com/test.jpg", ValidPrice, 10, ["Test"]);

            _basketRepositoryMock
                .Setup(x => x.FindByIdAsync(ValidBasketId))
                .ReturnsAsync(_basket);

            _customerRepositoryMock
                .Setup(x => x.FindByIdAsync(ValidCustomerId))
                .ReturnsAsync(_customer);

            _productRepositoryMock
                .Setup(x => x.FindByIdAsync(ValidProductId))
                .ReturnsAsync(_product);
        }

        public class HandleMethod : ConvertBasketToOrderCommandHandlerTests
        {
            [Fact]
            public async Task WithValidCommand_CreatesOrderAndClearsBasket()
            {
                // Arrange
                ConvertBasketToOrderCommand command = new()
                {
                    CustomerId = ValidCustomerId,
                    BasketId = ValidBasketId
                };

                // Act
                ConvertBasketToOrderCommandResponse response = await _handler.Handle(command, CancellationToken.None);

                // Assert
                Assert.NotEqual(Guid.Empty, response.OrderId);
                _orderRepositoryMock.Verify(x => x.SaveAsync(It.Is<Order>(o =>
                    o.Id == response.OrderId &&
                    o.CustomerId == ValidCustomerId &&
                    o.Items.Count == 1 &&
                    o.Items[0].ProductId == ValidProductId &&
                    o.Items[0].Price == ValidPrice &&
                    o.Items[0].Quantity == ValidQuantity)), Times.Once);
                _basketRepositoryMock.Verify(x => x.SaveAsync(It.Is<Basket>(b => !b.Items.Any())), Times.Once);
            }

            [Fact]
            public async Task WithEmptyCustomerId_ThrowsArgumentException()
            {
                // Arrange
                ConvertBasketToOrderCommand command = new()
                {
                    CustomerId = Guid.Empty,
                    BasketId = ValidBasketId
                };

                // Act & Assert
                ArgumentException exception = await Assert.ThrowsAsync<ArgumentException>(
                    () => _handler.Handle(command, CancellationToken.None));
                Assert.Equal("Customer ID cannot be empty (Parameter 'CustomerId')", exception.Message);
            }

            [Fact]
            public async Task WithCustomerMissingDetails_ThrowsCustomerDetailsNotFoundException()
            {
                // Arrange
                _customer.UpdateAddress(null);
                ConvertBasketToOrderCommand command = new()
                {
                    CustomerId = ValidCustomerId,
                    BasketId = ValidBasketId
                };

                // Act & Assert
                CustomerDetailsNotFoundException exception = await Assert.ThrowsAsync<CustomerDetailsNotFoundException>(
                    () => _handler.Handle(command, CancellationToken.None));
                Assert.Equal(ValidCustomerId, exception.CustomerId);
            }

            [Fact]
            public async Task WithOnlyAddress_UpdatesOnlyAddress()
            {
                // Arrange
                Address newAddress = new(
                    "456 Other Street",
                    "Suite 7",
                    "Manchester",
                    "Greater Manchester",
                    "M1 1AA"
                );

                ConvertBasketToOrderCommand command = new()
                {
                    CustomerId = ValidCustomerId,
                    BasketId = ValidBasketId
                };

                _customer.UpdateAddress(newAddress);

                // Act
                await _handler.Handle(command, CancellationToken.None);

                // Assert
                Assert.Equal(newAddress, _customer.Address);
            }

            [Fact]
            public async Task WithProductNotFound_ThrowsArgumentException()
            {
                // Arrange
                ConvertBasketToOrderCommand command = new()
                {
                    CustomerId = ValidCustomerId,
                    BasketId = ValidBasketId
                };

                _productRepositoryMock
                    .Setup(x => x.FindByIdAsync(ValidProductId))
                    .ReturnsAsync((Product?)null);

                // Act & Assert
                ArgumentException exception = await Assert.ThrowsAsync<ArgumentException>(
                    () => _handler.Handle(command, CancellationToken.None));
                Assert.Equal($"Product with ID {ValidProductId} not found. (Parameter 'BasketId')", exception.Message);
            }

            [Fact]
            public async Task WithPriceChange_ThrowsPriceChangeException()
            {
                // Arrange
                _product.UpdatePrice(ValidPrice + 1.00m);
                ConvertBasketToOrderCommand command = new()
                {
                    CustomerId = ValidCustomerId,
                    BasketId = ValidBasketId
                };

                // Act & Assert
                PriceChangeException exception = await Assert.ThrowsAsync<PriceChangeException>(
                    () => _handler.Handle(command, CancellationToken.None));
                Assert.Equal(ValidProductId, exception.ProductId);
                Assert.Equal(ValidPrice, exception.BasketPrice);
                Assert.Equal(ValidPrice + 1.00m, exception.ProductPrice);
            }
        }
    }
}