using Domain.Aggregates.Orders;
using Domain.Exceptions.Orders;
using Xunit;

namespace Domain.Tests.Aggregates.Orders;

public class OrderItemTests
{
    private static readonly Guid ValidProductId = Guid.NewGuid();
    private const Decimal ValidPrice = 10.00m;
    private const Int32 ValidQuantity = 5;
    private static readonly String ValidProductName = "Gaming PC";


    public class Constructor
    {
        [Fact]
        public void WithValidInputs_CreatesOrderItem()
        {
            // Act
            OrderItem orderItem = new OrderItem(ValidProductId, ValidPrice, ValidQuantity, ValidProductName);

            // Assert
            Assert.Equal(ValidProductId, orderItem.ProductId);
            Assert.Equal(ValidPrice, orderItem.Price);
            Assert.Equal(ValidQuantity, orderItem.Quantity);
            Assert.Equal(ValidPrice * ValidQuantity, orderItem.TotalPrice);
        }

        [Fact]
        public void WithEmptyProductId_ThrowsOrderValidationException()
        {
            // Act & Assert
            OrderValidationException exception = Assert.Throws<OrderValidationException>(
                () => new OrderItem(Guid.Empty, ValidPrice, ValidQuantity, ValidProductName));
            Assert.Equal("Product ID cannot be empty", exception.Message);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void WithInvalidPrice_ThrowsOrderValidationException(Decimal invalidPrice)
        {
            // Act & Assert
            OrderValidationException exception = Assert.Throws<OrderValidationException>(
                () => new OrderItem(ValidProductId, invalidPrice, ValidQuantity, ValidProductName));
            Assert.Equal("Price must be at least £0.01", exception.Message);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(100)]
        public void WithInvalidQuantity_ThrowsOrderValidationException(Int32 invalidQuantity)
        {
            // Act & Assert
            OrderValidationException exception = Assert.Throws<OrderValidationException>(
                () => new OrderItem(ValidProductId, ValidPrice, invalidQuantity, ValidProductName));
            Assert.Equal(
                invalidQuantity <= 0 ? "Quantity must be greater than zero" : "Quantity cannot exceed 99", 
                exception.Message);
        }
    }

    public class UpdateQuantityMethod
    {
        [Fact]
        public void WithValidQuantity_UpdatesQuantity()
        {
            // Arrange
            OrderItem orderItem = new OrderItem(ValidProductId, ValidPrice, ValidQuantity, ValidProductName);
            const Int32 newQuantity = 3;

            // Act
            orderItem.UpdateQuantity(newQuantity);

            // Assert
            Assert.Equal(newQuantity, orderItem.Quantity);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(100)]
        public void WithInvalidQuantity_ThrowsOrderValidationException(Int32 invalidQuantity)
        {
            // Arrange
            OrderItem orderItem = new OrderItem(ValidProductId, ValidPrice, ValidQuantity, ValidProductName);

            // Act & Assert
            Assert.Throws<OrderValidationException>(
                () => orderItem.UpdateQuantity(invalidQuantity));
        }
    }

    public class IncreaseQuantityMethod
    {
        [Fact]
        public void WithValidAmount_IncreasesQuantity()
        {
            // Arrange
            OrderItem orderItem = new OrderItem(ValidProductId, ValidPrice, ValidQuantity, ValidProductName);
            const Int32 increaseBy = 3;
            Int32 expectedQuantity = ValidQuantity + increaseBy;

            // Act
            orderItem.IncreaseQuantity(increaseBy);

            // Assert
            Assert.Equal(expectedQuantity, orderItem.Quantity);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void WithInvalidAmount_ThrowsOrderValidationException(Int32 invalidAmount)
        {
            // Arrange
            OrderItem orderItem = new OrderItem(ValidProductId, ValidPrice, ValidQuantity, ValidProductName);

            // Act & Assert
            OrderValidationException exception = Assert.Throws<OrderValidationException>(
                () => orderItem.IncreaseQuantity(invalidAmount));
            Assert.Equal("Quantity must be greater than zero", exception.Message);
        }

        [Fact]
        public void ExceedingMaxQuantity_ThrowsOrderValidationException()
        {
            // Arrange
            OrderItem orderItem = new OrderItem(ValidProductId, ValidPrice, 90, ValidProductName);

            // Act & Assert
            OrderValidationException exception = Assert.Throws<OrderValidationException>(
                () => orderItem.IncreaseQuantity(10));
            Assert.Equal("Quantity cannot exceed 99", exception.Message);
        }
    }

    public class DecreaseQuantityMethod
    {
        [Fact]
        public void WithValidAmount_DecreasesQuantity()
        {
            // Arrange
            OrderItem orderItem = new OrderItem(ValidProductId, ValidPrice, ValidQuantity, ValidProductName);
            const Int32 decreaseBy = 2;
            Int32 expectedQuantity = ValidQuantity - decreaseBy;

            // Act
            orderItem.DecreaseQuantity(decreaseBy);

            // Assert
            Assert.Equal(expectedQuantity, orderItem.Quantity);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void WithInvalidAmount_ThrowsOrderValidationException(Int32 invalidAmount)
        {
            // Arrange
            OrderItem orderItem = new OrderItem(ValidProductId, ValidPrice, ValidQuantity, ValidProductName);

            // Act & Assert
            OrderValidationException exception = Assert.Throws<OrderValidationException>(
                () => orderItem.DecreaseQuantity(invalidAmount));
            Assert.Equal("Quantity must be greater than zero", exception.Message);
        }

        [Fact]
        public void BelowOne_ThrowsOrderValidationException()
        {
            // Arrange
            OrderItem orderItem = new OrderItem(ValidProductId, ValidPrice, 3, ValidProductName);

            // Act & Assert
            OrderValidationException exception = Assert.Throws<OrderValidationException>(
                () => orderItem.DecreaseQuantity(3));
            Assert.Equal("Quantity must be greater than zero", exception.Message);
        }
    }

    public class UpdatePriceMethod
    {
        [Fact]
        public void WithValidPrice_UpdatesPrice()
        {
            // Arrange
            OrderItem orderItem = new OrderItem(ValidProductId, ValidPrice, ValidQuantity, ValidProductName);
            const Decimal newPrice = 15.00m;

            // Act
            orderItem.UpdatePrice(newPrice);

            // Assert
            Assert.Equal(newPrice, orderItem.Price);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void WithInvalidPrice_ThrowsOrderValidationException(Decimal invalidPrice)
        {
            // Arrange
            OrderItem orderItem = new OrderItem(ValidProductId, ValidPrice, ValidQuantity, ValidProductName);

            // Act & Assert
            OrderValidationException exception = Assert.Throws<OrderValidationException>(
                () => orderItem.UpdatePrice(invalidPrice));
            Assert.Equal("Price must be at least £0.01", exception.Message);
        }
    }
} 