using Domain.Aggregates.Basket;
using Domain.Exceptions.Baskets;
using Xunit;

namespace Domain.Tests.Aggregates.Baskets;

public class BasketItemTests
{
    private static readonly Guid ValidProductId = Guid.NewGuid();
    private static readonly Int32 ValidQuantity = 1;
    private static readonly Decimal ValidPrice = 10.00m;
    private static readonly String ValidProductName = "Test Product";
    
    private static BasketItem CreateValidBasketItem(
        Guid? productId = null,
        int? quantity = null,
        Decimal? price = null,
        String? name = null)
    {
        return new BasketItem(
            productId ?? ValidProductId,
            quantity ?? ValidQuantity,
            price ?? ValidPrice,
            name ?? ValidProductName
        );
    }

    public class Constructor
    {
        [Fact]
        public void GivenValidParameters_ShouldCreateBasketItem()
        {
            // Arrange
            Guid productId = ValidProductId;
            Int32 quantity = ValidQuantity;
            Decimal price = ValidPrice;
            String name = ValidProductName;

            // Act
            BasketItem basketItem = new BasketItem(productId, quantity, price, name);

            // Assert
            Assert.Equal(productId, basketItem.ProductId);
            Assert.Equal(quantity, basketItem.Quantity);
            Assert.Equal(price, basketItem.Price);
            Assert.Equal(price * quantity, basketItem.TotalPrice);
        }

        [Fact]
        public void GivenEmptyProductId_ShouldThrowBasketItemValidationException()
        {
            BasketItemValidationException exception = Assert.Throws<BasketItemValidationException>(() => 
                new BasketItem(Guid.Empty, ValidQuantity, ValidPrice, ValidProductName));
            
            Assert.Equal("Product ID cannot be empty", exception.Message);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void GivenInvalidQuantity_ShouldThrowBasketItemValidationException(Int32 invalidQuantity)
        {
            BasketItemValidationException exception = Assert.Throws<BasketItemValidationException>(() => 
                new BasketItem(ValidProductId, invalidQuantity, ValidPrice, ValidProductName));
            
            Assert.Equal("Quantity must be greater than zero", exception.Message);
        }

        [Fact]
        public void GivenQuantityAboveMax_ShouldThrowBasketItemValidationException()
        {
            BasketItemValidationException exception = Assert.Throws<BasketItemValidationException>(() => 
                new BasketItem(ValidProductId, 100, ValidPrice, ValidProductName));
            
            Assert.Equal("Quantity cannot exceed 99", exception.Message);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-0.01)]
        [InlineData(-1)]
        public void GivenInvalidPrice_ShouldThrowBasketItemValidationException(Decimal invalidPrice)
        {
            BasketItemValidationException exception = Assert.Throws<BasketItemValidationException>(() => 
                new BasketItem(ValidProductId, ValidQuantity, invalidPrice, ValidProductName));
            
            Assert.Equal("Price must be at least £0.01", exception.Message);
        }
    }

    public class UpdateQuantity
    {
        [Theory]
        [InlineData(1)]
        [InlineData(50)]
        [InlineData(99)]
        public void GivenValidQuantity_ShouldUpdateQuantity(Int32 newQuantity)
        {
            // Arrange
            BasketItem basketItem = CreateValidBasketItem();

            // Act
            basketItem.UpdateQuantity(newQuantity);

            // Assert
            Assert.Equal(newQuantity, basketItem.Quantity);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void GivenInvalidQuantity_ShouldThrowBasketItemValidationException(Int32 invalidQuantity)
        {
            // Arrange
            BasketItem basketItem = CreateValidBasketItem();

            // Act & Assert
            BasketItemValidationException exception = Assert.Throws<BasketItemValidationException>(() => 
                basketItem.UpdateQuantity(invalidQuantity));
            
            Assert.Equal("Quantity must be greater than zero", exception.Message);
        }

        [Fact]
        public void GivenQuantityAboveMax_ShouldThrowBasketItemValidationException()
        {
            // Arrange
            BasketItem basketItem = CreateValidBasketItem();

            // Act & Assert
            BasketItemValidationException exception = Assert.Throws<BasketItemValidationException>(() => 
                basketItem.UpdateQuantity(100));
            
            Assert.Equal("Quantity cannot exceed 99", exception.Message);
        }
    }

    public class IncreaseQuantity
    {
        [Theory]
        [InlineData(1, 1)]
        [InlineData(1, 98)]
        [InlineData(50, 49)]
        public void GivenValidIncrease_ShouldIncreaseQuantity(Int32 initialQuantity, Int32 increase)
        {
            // Arrange
            BasketItem basketItem = CreateValidBasketItem(quantity: initialQuantity);

            // Act
            basketItem.IncreaseQuantity(increase);

            // Assert
            Assert.Equal(initialQuantity + increase, basketItem.Quantity);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void GivenInvalidIncrease_ShouldThrowBasketItemValidationException(Int32 invalidIncrease)
        {
            // Arrange
            BasketItem basketItem = CreateValidBasketItem();

            // Act & Assert
            BasketItemValidationException exception = Assert.Throws<BasketItemValidationException>(() => 
                basketItem.IncreaseQuantity(invalidIncrease));
            
            Assert.Equal("Quantity must be greater than zero", exception.Message);
        }

        [Theory]
        [InlineData(90, 10)]
        [InlineData(99, 1)]
        public void GivenIncreaseAboveMax_ShouldThrowBasketItemValidationException(Int32 initialQuantity, Int32 increase)
        {
            // Arrange
            BasketItem basketItem = CreateValidBasketItem(quantity: initialQuantity);

            // Act & Assert
            BasketItemValidationException exception = Assert.Throws<BasketItemValidationException>(() => 
                basketItem.IncreaseQuantity(increase));
            
            Assert.Equal("Quantity cannot exceed 99", exception.Message);
        }
    }

    public class DecreaseQuantity
    {
        [Theory]
        [InlineData(2, 1)]
        [InlineData(99, 98)]
        [InlineData(50, 49)]
        public void GivenValidDecrease_ShouldDecreaseQuantity(Int32 initialQuantity, Int32 decrease)
        {
            // Arrange
            BasketItem basketItem = CreateValidBasketItem(quantity: initialQuantity);

            // Act
            basketItem.DecreaseQuantity(decrease);

            // Assert
            Assert.Equal(initialQuantity - decrease, basketItem.Quantity);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void GivenInvalidDecrease_ShouldThrowBasketItemValidationException(Int32 invalidDecrease)
        {
            // Arrange
            BasketItem basketItem = CreateValidBasketItem();

            // Act & Assert
            BasketItemValidationException exception = Assert.Throws<BasketItemValidationException>(() => 
                basketItem.DecreaseQuantity(invalidDecrease));
            
            Assert.Equal("Quantity must be greater than zero", exception.Message);
        }

        [Theory]
        [InlineData(1, 1)]  // Would result in 0
        [InlineData(5, 10)] // Would result in negative
        public void GivenDecreaseResultingInInvalidQuantity_ShouldThrowBasketItemValidationException(
            Int32 initialQuantity, Int32 decrease)
        {
            // Arrange
            BasketItem basketItem = CreateValidBasketItem(quantity: initialQuantity);

            // Act & Assert
            BasketItemValidationException exception = Assert.Throws<BasketItemValidationException>(() => 
                basketItem.DecreaseQuantity(decrease));
            
            Assert.Equal("Quantity must be greater than zero", exception.Message);
        }
    }

    public class UpdatePrice
    {
        [Theory]
        [InlineData(0.01)]
        [InlineData(1.00)]
        [InlineData(999.99)]
        public void GivenValidPrice_ShouldUpdatePrice(Decimal newPrice)
        {
            // Arrange
            BasketItem basketItem = CreateValidBasketItem();

            // Act
            basketItem.UpdatePrice(newPrice);

            // Assert
            Assert.Equal(newPrice, basketItem.Price);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-0.01)]
        [InlineData(-1.00)]
        public void GivenInvalidPrice_ShouldThrowBasketItemValidationException(Decimal invalidPrice)
        {
            // Arrange
            BasketItem basketItem = CreateValidBasketItem();

            // Act & Assert
            BasketItemValidationException exception = Assert.Throws<BasketItemValidationException>(() => 
                basketItem.UpdatePrice(invalidPrice));
            
            Assert.Equal("Price must be at least £0.01", exception.Message);
        }
    }
} 