using Application.DataTransferObjects;
using Application.Queries.Orders;
using Domain.Aggregates.Orders;
using Domain.Repositories;
using Moq;
using Xunit;

namespace Application.Tests.Queries.Orders;

public class GetOrderDetailsQueryHandlerTests
{
    private static readonly Guid ValidCustomerId = Guid.NewGuid();
    private static readonly Guid ValidOrderId = Guid.NewGuid();
    private static readonly Guid ValidProductId = Guid.NewGuid();
    private static readonly String ValidProductName = "Gaming PC";
    private static readonly Decimal ValidPrice = 10.00m;
    private static readonly Int32 ValidQuantity = 1;

    private readonly Mock<IOrderRepository> _orderRepositoryMock;
    private readonly GetOrderDetailsQueryHandler _handler;
    private readonly Order _order;

    public GetOrderDetailsQueryHandlerTests()
    {
        _orderRepositoryMock = new Mock<IOrderRepository>();
        _handler = new GetOrderDetailsQueryHandler(_orderRepositoryMock.Object);

        _order = new Order(ValidOrderId, ValidCustomerId);
        _order.AddItem(new OrderItem(ValidProductId, ValidPrice, ValidQuantity, ValidProductName));
    }

    public class HandleMethod : GetOrderDetailsQueryHandlerTests
    {
        [Fact]
        public async Task WithExistingOrder_ReturnsCorrectResponse()
        {
            // Arrange
            GetOrderDetailsQuery query = new GetOrderDetailsQuery 
            { 
                OrderId = ValidOrderId,
                CustomerId = ValidCustomerId
            };
            _orderRepositoryMock
                .Setup(x => x.FindByIdAsync(ValidOrderId))
                .ReturnsAsync(_order);

            // Act
            GetOrderDetailsQueryResponse? response = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(response);
            Assert.NotNull(response.OrderDetails);
            
            OrderDetails? order = response.OrderDetails;
            Assert.Equal(ValidOrderId, order.Id);
            Assert.Equal(ValidCustomerId, order.CustomerId);
            Assert.Equal(OrderProcessingStatus.Pending, order.Status);
            Assert.Equal(ValidPrice * ValidQuantity, order.TotalPrice);
            
            Assert.Single(order.Items);
            OrderItemDetails item = order.Items[0];
            Assert.Equal(ValidProductId, item.ProductId);
            Assert.Equal(ValidQuantity, item.Quantity);
            Assert.Equal(ValidPrice, item.Price);
            Assert.Equal(ValidPrice * ValidQuantity, item.TotalPrice);
        }

        [Fact]
        public async Task WithNonExistingOrder_ReturnsNull()
        {
            // Arrange
            GetOrderDetailsQuery query = new GetOrderDetailsQuery 
            { 
                OrderId = ValidOrderId,
                CustomerId = ValidCustomerId
            };
            _orderRepositoryMock
                .Setup(x => x.FindByIdAsync(ValidOrderId))
                .ReturnsAsync((Order?)null);

            // Act
            GetOrderDetailsQueryResponse response = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.Null(response.OrderDetails);
        }

        [Fact]
        public async Task WithWrongCustomerId_ReturnsNull()
        {
            // Arrange
            GetOrderDetailsQuery query = new GetOrderDetailsQuery 
            { 
                OrderId = ValidOrderId,
                CustomerId = Guid.NewGuid() // Different customer ID
            };
            _orderRepositoryMock
                .Setup(x => x.FindByIdAsync(ValidOrderId))
                .ReturnsAsync(_order);

            // Act
            GetOrderDetailsQueryResponse response = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.Null(response.OrderDetails);
        }
    }
} 