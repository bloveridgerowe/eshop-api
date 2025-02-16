using Domain.Aggregates.Orders;
using Domain.Events.Orders;
using Domain.Exceptions.Orders;
using Xunit;

namespace Domain.Tests.Aggregates.Orders;

public class OrderTests
{
    private static readonly Guid ValidId = Guid.NewGuid();
    private static readonly Guid ValidCustomerId = Guid.NewGuid();
    private static readonly String ValidProductName = "Gaming PC";
    private static readonly OrderItem ValidOrderItem = new(Guid.NewGuid(), 10.00m, 1, ValidProductName);
    
    public class Constructor
    {
        [Fact]
        public void WithValidInputs_CreatesOrder()
        {
            // Act
            Order order = new Order(ValidId, ValidCustomerId);

            // Assert
            Assert.Equal(ValidId, order.Id);
            Assert.Equal(ValidCustomerId, order.CustomerId);
            Assert.Equal(OrderProcessingStatus.Pending, order.Status);
            Assert.Empty(order.Items);
            Assert.Equal(0m, order.TotalPrice);
        }

        [Fact]
        public void WithEmptyId_ThrowsOrderValidationException()
        {
            // Act & Assert
            OrderValidationException exception = Assert.Throws<OrderValidationException>(
                () => new Order(Guid.Empty, ValidCustomerId));
            Assert.Equal("Order ID cannot be empty", exception.Message);
        }

        [Fact]
        public void WithEmptyCustomerId_ThrowsOrderValidationException()
        {
            // Act & Assert
            OrderValidationException exception = Assert.Throws<OrderValidationException>(
                () => new Order(ValidId, Guid.Empty));
            Assert.Equal("Customer ID cannot be empty", exception.Message);
        }
    }

    public class AddItemMethod
    {
        [Fact]
        public void WithNewItem_AddsToItems()
        {
            // Arrange
            Order order = new Order(ValidId, ValidCustomerId);

            // Act
            order.AddItem(ValidOrderItem);

            // Assert
            Assert.Single(order.Items);
            Assert.Equal(ValidOrderItem, order.Items[0]);
            Assert.Equal(ValidOrderItem.TotalPrice, order.TotalPrice);
        }

        [Fact]
        public void WithExistingItem_IncreasesQuantity()
        {
            // Arrange
            Order order = new Order(ValidId, ValidCustomerId);
            order.AddItem(ValidOrderItem);
            OrderItem additionalQuantity = new OrderItem(ValidOrderItem.ProductId, ValidOrderItem.Price, 2, ValidProductName);

            // Act
            order.AddItem(additionalQuantity);

            // Assert
            Assert.Single(order.Items);
            Assert.Equal(3, order.Items[0].Quantity);
            Assert.Equal(ValidOrderItem.Price * 3, order.TotalPrice);
        }

        [Fact]
        public void WhenNotPending_ThrowsOrderValidationException()
        {
            // Arrange
            Order order = new Order(ValidId, ValidCustomerId);
            order.AddItem(ValidOrderItem);
            order.MarkAsShipped();

            // Act & Assert
            OrderValidationException exception = Assert.Throws<OrderValidationException>(
                () => order.AddItem(new OrderItem(Guid.NewGuid(), 20.00m, 1, ValidProductName)));
            Assert.Equal("Can only modify pending orders", exception.Message);
        }

        [Fact]
        public void WithTooManyItems_ThrowsOrderValidationException()
        {
            // Arrange
            Order order = new Order(ValidId, ValidCustomerId);
            for (Int32 i = 0; i < 99; i++)
            {
                order.AddItem(new OrderItem(Guid.NewGuid(), 10.00m, 1, ValidProductName));
            }

            // Act & Assert
            OrderValidationException exception = Assert.Throws<OrderValidationException>(
                () => order.AddItem(new OrderItem(Guid.NewGuid(), 10.00m, 1, ValidProductName)));
            Assert.Equal("Order cannot have more than 99 unique items", exception.Message);
        }
    }

    public class RemoveItemMethod
    {
        [Fact]
        public void WithExistingItem_RemovesFromItems()
        {
            // Arrange
            Order order = new Order(ValidId, ValidCustomerId);
            order.AddItem(ValidOrderItem);

            // Act
            order.RemoveItem(ValidOrderItem.ProductId);

            // Assert
            Assert.Empty(order.Items);
            Assert.Equal(0m, order.TotalPrice);
        }

        [Fact]
        public void WithNonExistingItem_DoesNothing()
        {
            // Arrange
            Order order = new Order(ValidId, ValidCustomerId);
            order.AddItem(ValidOrderItem);

            // Act
            order.RemoveItem(Guid.NewGuid());

            // Assert
            Assert.Single(order.Items);
            Assert.Equal(ValidOrderItem.TotalPrice, order.TotalPrice);
        }

        [Fact]
        public void WhenNotPending_ThrowsOrderValidationException()
        {
            // Arrange
            Order order = new Order(ValidId, ValidCustomerId);
            order.AddItem(ValidOrderItem);
            order.MarkAsShipped();

            // Act & Assert
            OrderValidationException exception = Assert.Throws<OrderValidationException>(
                () => order.RemoveItem(ValidOrderItem.ProductId));
            Assert.Equal("Can only modify pending orders", exception.Message);
        }

        [Fact]
        public void WithEmptyProductId_ThrowsOrderValidationException()
        {
            // Arrange
            Order order = new Order(ValidId, ValidCustomerId);

            // Act & Assert
            OrderValidationException exception = Assert.Throws<OrderValidationException>(
                () => order.RemoveItem(Guid.Empty));
            Assert.Equal("Product ID cannot be empty", exception.Message);
        }
    }

    public class MarkAsShippedMethod
    {
        [Fact]
        public void WhenPendingWithItems_MarksAsShipped()
        {
            // Arrange
            Order order = new Order(ValidId, ValidCustomerId);
            order.AddItem(ValidOrderItem);

            // Act
            order.MarkAsShipped();

            // Assert
            Assert.Equal(OrderProcessingStatus.Shipped, order.Status);
            OrderStatusChangedEvent? @event = Assert.Single(order.Events) as OrderStatusChangedEvent;
            Assert.NotNull(@event);
            Assert.Equal(OrderProcessingStatus.Shipped, @event!.OrderStatus);
        }

        [Fact]
        public void WhenNotPending_ThrowsOrderStatusException()
        {
            // Arrange
            Order order = new Order(ValidId, ValidCustomerId);
            order.AddItem(ValidOrderItem);
            order.MarkAsShipped();

            // Act & Assert
            OrderStatusException exception = Assert.Throws<OrderStatusException>(
                () => order.MarkAsShipped());
            Assert.Equal("Only pending orders can be marked as shipped", exception.Message);
            Assert.Equal(OrderProcessingStatus.Shipped, exception.CurrentStatus);
            Assert.Equal(OrderProcessingStatus.Shipped, exception.AttemptedStatus);
        }

        [Fact]
        public void WhenEmpty_ThrowsOrderValidationException()
        {
            // Arrange
            Order order = new Order(ValidId, ValidCustomerId);

            // Act & Assert
            OrderValidationException exception = Assert.Throws<OrderValidationException>(
                () => order.MarkAsShipped());
            Assert.Equal("Cannot ship an empty order", exception.Message);
        }
    }

    public class MarkAsDeliveredMethod
    {
        [Fact]
        public void WhenShipped_MarksAsDelivered()
        {
            // Arrange
            Order order = new Order(ValidId, ValidCustomerId);
            order.AddItem(ValidOrderItem);
            order.MarkAsShipped();

            // Act
            order.MarkAsDelivered();

            // Assert
            Assert.Equal(OrderProcessingStatus.Delivered, order.Status);
            OrderStatusChangedEvent? @event = order.Events.Last() as OrderStatusChangedEvent;
            Assert.NotNull(@event);
            Assert.Equal(OrderProcessingStatus.Delivered, @event!.OrderStatus);
        }
    }

    public class MarkAsCancelledMethod
    {
        [Fact]
        public void WhenPending_MarksAsCancelled()
        {
            // Arrange
            Order order = new Order(ValidId, ValidCustomerId);
            order.AddItem(ValidOrderItem);

            // Act
            order.MarkAsCancelled();

            // Assert
            Assert.Equal(OrderProcessingStatus.Canceled, order.Status);
            OrderStatusChangedEvent? @event = Assert.Single(order.Events) as OrderStatusChangedEvent;
            Assert.NotNull(@event);
            Assert.Equal(OrderProcessingStatus.Canceled, @event!.OrderStatus);
        }

        [Fact]
        public void WhenNotPending_ThrowsOrderStatusException()
        {
            // Arrange
            Order order = new Order(ValidId, ValidCustomerId);
            order.AddItem(ValidOrderItem);
            order.MarkAsShipped();

            // Act & Assert
            OrderStatusException exception = Assert.Throws<OrderStatusException>(
                () => order.MarkAsCancelled());
            Assert.Equal("Only pending orders can be canceled", exception.Message);
            Assert.Equal(OrderProcessingStatus.Shipped, exception.CurrentStatus);
            Assert.Equal(OrderProcessingStatus.Canceled, exception.AttemptedStatus);
        }
    }
} 