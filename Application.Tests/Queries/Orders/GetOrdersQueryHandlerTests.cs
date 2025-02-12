using Application.DataTransferObjects;
using Application.Queries.Orders;
using Domain.Aggregates.Orders;
using Domain.Repositories;
using Moq;
using Xunit;

namespace Application.Tests.Queries.Orders;

public class GetOrdersQueryHandlerTests
{
    private static readonly Guid ValidCustomerId = Guid.NewGuid();
    private static readonly Guid ValidOrderId = Guid.NewGuid();
    private static readonly Guid ValidProductId = Guid.NewGuid();
    private static readonly String ValidProductName = "Gaming PC";
    private static readonly Decimal ValidPrice = 10.00m;
    private static readonly Int32 ValidQuantity = 1;

    private readonly Mock<IOrderRepository> _orderRepositoryMock;
    private readonly GetOrdersQueryHandler _handler;
    private readonly List<Order> _orders;

    public GetOrdersQueryHandlerTests()
    {
        _orderRepositoryMock = new Mock<IOrderRepository>();
        _handler = new GetOrdersQueryHandler(_orderRepositoryMock.Object);

        Order order = new Order(ValidOrderId, ValidCustomerId);
        order.AddItem(new OrderItem(ValidProductId, ValidPrice, ValidQuantity, ValidProductName));
        _orders = [order];
    }

    public class HandleMethod : GetOrdersQueryHandlerTests
    {
        [Fact]
        public async Task WithExistingOrders_ReturnsCorrectResponse()
        {
            // Arrange
            GetOrdersQuery query = new GetOrdersQuery { CustomerId = ValidCustomerId };
            _orderRepositoryMock
                .Setup(x => x.GetOrdersForCustomerAsync(ValidCustomerId))
                .ReturnsAsync(_orders);

            // Act
            GetOrdersQueryResponse response = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(response);
            Assert.Single(response.Orders);
            
            OrderDetails order = response.Orders[0];
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
        public async Task WithNoOrders_ReturnsEmptyList()
        {
            // Arrange
            GetOrdersQuery query = new GetOrdersQuery { CustomerId = ValidCustomerId };
            _orderRepositoryMock
                .Setup(x => x.GetOrdersForCustomerAsync(ValidCustomerId))
                .ReturnsAsync([]);

            // Act
            GetOrdersQueryResponse response = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(response);
            Assert.Empty(response.Orders);
        }

        [Fact]
        public async Task WithMultipleOrders_ReturnsAllOrders()
        {
            // Arrange
            Order order2 = new Order(Guid.NewGuid(), ValidCustomerId);
            order2.AddItem(new OrderItem(ValidProductId, ValidPrice * 2, ValidQuantity * 2, ValidProductName));
            List<Order> orders = [_orders[0], order2];
            
            GetOrdersQuery query = new GetOrdersQuery { CustomerId = ValidCustomerId };
            _orderRepositoryMock
                .Setup(x => x.GetOrdersForCustomerAsync(ValidCustomerId))
                .ReturnsAsync(orders);

            // Act
            GetOrdersQueryResponse response = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(response);
            Assert.Equal(2, response.Orders.Count);
            Assert.Equal(ValidPrice * ValidQuantity, response.Orders[0].TotalPrice);
            Assert.Equal(ValidPrice * 2 * ValidQuantity * 2, response.Orders[1].TotalPrice);
        }
    }
} 