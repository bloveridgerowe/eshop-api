using Domain.Aggregates.Basket;
using Domain.Exceptions.Baskets;
using Xunit;

namespace Domain.Tests.Aggregates.Baskets;

public class BasketTests
{
    private static readonly Guid ValidId = Guid.NewGuid();
    private static readonly Guid ValidCustomerId = Guid.NewGuid();
    private static readonly DateTime ValidCreatedAt = DateTime.UtcNow.AddMinutes(-5);
    private static readonly DateTime ValidUpdatedAt = DateTime.UtcNow;
    
    private static Basket CreateValidBasket(
        Guid? id = null,
        Guid? customerId = null,
        DateTime? createdAt = null,
        DateTime? updatedAt = null)
    {
        return new Basket(
            id ?? ValidId,
            customerId ?? ValidCustomerId,
            createdAt ?? ValidCreatedAt,
            updatedAt ?? ValidUpdatedAt
        );
    }
    
    private static BasketItem CreateValidBasketItem(
        Guid? productId = null,
        Int32 quantity = 1,
        Decimal price = 10.00m,
        String name = "Test Product")
    {
        return new BasketItem(
            productId ?? Guid.NewGuid(),
            quantity,
            price,
            name
        );
    }

    public class Constructor
    {
        [Fact]
        public void GivenValidParameters_ShouldCreateBasket()
        {
            // Arrange
            Guid id = Guid.NewGuid();
            Guid customerId = Guid.NewGuid();
            DateTime createdAt = DateTime.UtcNow.AddMinutes(-5);
            DateTime updatedAt = DateTime.UtcNow;

            // Act
            Basket basket = new Basket(id, customerId, createdAt, updatedAt);

            // Assert
            Assert.Equal(id, basket.Id);
            Assert.Equal(customerId, basket.CustomerId);
            Assert.Equal(createdAt, basket.CreatedAt);
            Assert.Equal(updatedAt, basket.UpdatedAt);
            Assert.Empty(basket.Items);
            Assert.Equal(0m, basket.TotalPrice);
        }

        [Fact]
        public void GivenEmptyCustomerId_ShouldThrowBasketValidationException()
        {
            BasketValidationException exception = Assert.Throws<BasketValidationException>(() => 
                new Basket(ValidId, Guid.Empty, ValidCreatedAt, ValidUpdatedAt));
            
            Assert.Equal("Customer ID cannot be empty", exception.Message);
        }

        [Fact]
        public void GivenCreatedDateLaterThanUpdatedDate_ShouldThrowBasketValidationException()
        {
            BasketValidationException exception = Assert.Throws<BasketValidationException>(() => 
                new Basket(ValidId, ValidCustomerId, DateTime.UtcNow, DateTime.UtcNow.AddMinutes(-5)));
            
            Assert.Equal("Created date cannot be later than updated date", exception.Message);
        }

        [Fact]
        public void GivenNullDates_ShouldUseCurrentUtcTime()
        {
            // Act
            Basket basket = new Basket(ValidId, ValidCustomerId);

            // Assert
            Assert.True(basket.CreatedAt <= DateTime.UtcNow);
            Assert.True(basket.UpdatedAt <= DateTime.UtcNow);
            Assert.Equal(basket.CreatedAt, basket.UpdatedAt);
        }
    }

    public class AddItem
    {
        [Fact]
        public void GivenValidNewItem_ShouldAddToBasket()
        {
            // Arrange
            Basket basket = CreateValidBasket();
            BasketItem item = CreateValidBasketItem();

            // Act
            basket.AddItem(item);

            // Assert
            Assert.Single(basket.Items);
            Assert.Equal(item.ProductId, basket.Items[0].ProductId);
            Assert.Equal(item.TotalPrice, basket.TotalPrice);
        }

        [Fact]
        public void GivenExistingProductId_ShouldIncreaseQuantity()
        {
            // Arrange
            Basket basket = CreateValidBasket();
            Guid productId = Guid.NewGuid();
            BasketItem item1 = CreateValidBasketItem(productId, quantity: 2);
            BasketItem item2 = CreateValidBasketItem(productId, quantity: 3);

            // Act
            basket.AddItem(item1);
            basket.AddItem(item2);

            // Assert
            Assert.Single(basket.Items);
            Assert.Equal(5, basket.Items[0].Quantity);
        }

        [Fact]
        public void GivenNullItem_ShouldThrowArgumentNullException()
        {
            // Arrange
            Basket basket = CreateValidBasket();

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => basket.AddItem(null!));
        }

        [Fact]
        public void GivenBasketIsFull_ShouldThrowBasketValidationException()
        {
            // Arrange
            Basket basket = CreateValidBasket();
            for (Int32 i = 0; i < 50; i++)
            {
                basket.AddItem(CreateValidBasketItem());
            }

            // Act & Assert
            BasketValidationException exception = Assert.Throws<BasketValidationException>(() => 
                basket.AddItem(CreateValidBasketItem()));
            
            Assert.Equal("Basket cannot contain more than 50 items", exception.Message);
        }
    }

    public class AddItems
    {
        [Fact]
        public void GivenValidItems_ShouldAddAllToBasket()
        {
            // Arrange
            Basket basket = CreateValidBasket();
            BasketItem[] items =
            [
                CreateValidBasketItem(),
                CreateValidBasketItem(),
                CreateValidBasketItem()
            ];

            // Act
            basket.AddItems(items);

            // Assert
            Assert.Equal(3, basket.Items.Count);
            Assert.Equal(items.Sum(i => i.TotalPrice), basket.TotalPrice);
        }

        [Fact]
        public void GivenNullCollection_ShouldThrowArgumentNullException()
        {
            // Arrange
            Basket basket = CreateValidBasket();

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => basket.AddItems(null!));
        }

        [Fact]
        public void GivenEmptyCollection_ShouldThrowBasketValidationException()
        {
            // Arrange
            Basket basket = CreateValidBasket();

            // Act & Assert
            BasketValidationException exception = Assert.Throws<BasketValidationException>(() => 
                basket.AddItems(Array.Empty<BasketItem>()));
            
            Assert.Equal("Items collection cannot be empty", exception.Message);
        }

        [Fact]
        public void GivenTooManyItems_ShouldThrowBasketValidationException()
        {
            // Arrange
            Basket basket = CreateValidBasket();
            List<BasketItem> items = Enumerable.Range(0, 51).Select(_ => CreateValidBasketItem()).ToList();

            // Act & Assert
            BasketValidationException exception = Assert.Throws<BasketValidationException>(() => 
                basket.AddItems(items));
            
            Assert.Contains("Cannot add 51 items", exception.Message);
        }
    }

    public class RemoveItem
    {
        [Fact]
        public void GivenExistingProductId_ShouldRemoveFromBasket()
        {
            // Arrange
            Basket basket = CreateValidBasket();
            BasketItem item = CreateValidBasketItem();
            basket.AddItem(item);

            // Act
            basket.RemoveItem(item.ProductId);

            // Assert
            Assert.Empty(basket.Items);
            Assert.Equal(0m, basket.TotalPrice);
        }

        [Fact]
        public void GivenEmptyProductId_ShouldThrowBasketValidationException()
        {
            // Arrange
            Basket basket = CreateValidBasket();

            // Act & Assert
            BasketValidationException exception = Assert.Throws<BasketValidationException>(() => 
                basket.RemoveItem(Guid.Empty));
            
            Assert.Equal("Product ID cannot be empty", exception.Message);
        }

        [Fact]
        public void GivenNonExistentProductId_ShouldThrowBasketValidationException()
        {
            // Arrange
            Basket basket = CreateValidBasket();
            Guid productId = Guid.NewGuid();

            // Act & Assert
            BasketValidationException exception = Assert.Throws<BasketValidationException>(() => 
                basket.RemoveItem(productId));
            
            Assert.Equal($"Product with ID {productId} not found in basket", exception.Message);
        }
    }

    public class UpdateItemQuantity
    {
        [Fact]
        public void GivenValidQuantity_ShouldUpdateItemQuantity()
        {
            // Arrange
            Basket basket = CreateValidBasket();
            BasketItem item = CreateValidBasketItem(quantity: 1);
            basket.AddItem(item);

            // Act
            basket.UpdateItemQuantity(item.ProductId, 3);

            // Assert
            Assert.Equal(3, basket.Items[0].Quantity);
        }

        [Fact]
        public void GivenEmptyProductId_ShouldThrowBasketValidationException()
        {
            // Arrange
            Basket basket = CreateValidBasket();

            // Act & Assert
            BasketValidationException exception = Assert.Throws<BasketValidationException>(() => 
                basket.UpdateItemQuantity(Guid.Empty, 1));
            
            Assert.Equal("Product ID cannot be empty", exception.Message);
        }

        [Fact]
        public void GivenNonExistentProductId_ShouldThrowBasketValidationException()
        {
            // Arrange
            Basket basket = CreateValidBasket();
            Guid productId = Guid.NewGuid();

            // Act & Assert
            BasketValidationException exception = Assert.Throws<BasketValidationException>(() => 
                basket.UpdateItemQuantity(productId, 1));
            
            Assert.Equal($"Product with ID {productId} not found in basket", exception.Message);
        }
    }

    public class UpdateItemPrice
    {
        [Fact]
        public void GivenValidPrice_ShouldUpdateItemPrice()
        {
            // Arrange
            Basket basket = CreateValidBasket();
            BasketItem item = CreateValidBasketItem(price: 10.00m);
            basket.AddItem(item);

            // Act
            basket.UpdateItemPrice(item.ProductId, 15.00m);

            // Assert
            Assert.Equal(15.00m, basket.Items[0].Price);
        }

        [Fact]
        public void GivenEmptyProductId_ShouldThrowBasketValidationException()
        {
            // Arrange
            Basket basket = CreateValidBasket();

            // Act & Assert
            BasketValidationException exception = Assert.Throws<BasketValidationException>(() => 
                basket.UpdateItemPrice(Guid.Empty, 10.00m));
            
            Assert.Equal("Product ID cannot be empty", exception.Message);
        }

        [Fact]
        public void GivenNonExistentProductId_ShouldThrowBasketValidationException()
        {
            // Arrange
            Basket basket = CreateValidBasket();
            Guid productId = Guid.NewGuid();

            // Act & Assert
            BasketValidationException exception = Assert.Throws<BasketValidationException>(() => 
                basket.UpdateItemPrice(productId, 10.00m));
            
            Assert.Equal($"Product with ID {productId} not found in basket", exception.Message);
        }
    }

    public class ClearItems
    {
        [Fact]
        public void GivenBasketWithItems_ShouldClearAllItems()
        {
            // Arrange
            Basket basket = CreateValidBasket();
            basket.AddItem(CreateValidBasketItem());
            basket.AddItem(CreateValidBasketItem());

            // Act
            basket.ClearItems();

            // Assert
            Assert.Empty(basket.Items);
            Assert.Equal(0m, basket.TotalPrice);
        }
    }
} 