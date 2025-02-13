 using Domain.Entities;
using Domain.Exceptions.Products;
using Xunit;

namespace Domain.Tests.Entities;

public class ProductTests
{
    private static readonly Guid ValidId = Guid.NewGuid();
    private static readonly String ValidName = "Test Product";
    private static readonly String ValidDescription = "Test Description";
    private static readonly String ValidImageUrl = "https://example.com/image.jpg";
    private static readonly Decimal ValidPrice = 10.00m;
    private static readonly List<String> ValidCategories = ["Category1"];
    
    private static Product CreateValidProduct(
        Guid? id = null,
        String? name = null,
        String? description = null,
        bool featured = false,
        String? imageUrl = null,
        Decimal? price = null,
        Int32 stock = 10,
        List<String>? categories = null)
    {
        return new Product(
            id ?? ValidId,
            name ?? ValidName,
            description ?? ValidDescription,
            featured,
            imageUrl ?? ValidImageUrl,
            price ?? ValidPrice,
            stock,
            categories ?? ValidCategories
        );
    }

    public class Constructor
    {
        [Fact]
        public void GivenValidParameters_ShouldCreateProduct()
        {
            // Act
            Product product = new Product(ValidId, ValidName, ValidDescription, false, ValidImageUrl, ValidPrice, 5, ValidCategories);

            // Assert
            Assert.Equal(ValidId, product.Id);
            Assert.Equal(ValidName, product.Name);
            Assert.Equal(ValidDescription, product.Description);
            Assert.False(product.Featured);
            Assert.Equal(ValidImageUrl, product.ImageUrl);
            Assert.Equal(ValidPrice, product.Price);
            Assert.Equal(5, product.Stock);
            Assert.Single(product.Categories);
            Assert.Equal(ValidCategories[0], product.Categories[0]);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void GivenInvalidName_ShouldThrowProductValidationException(String? invalidName)
        {
            ProductValidationException exception = Assert.Throws<ProductValidationException>(() => 
                new Product(ValidId, invalidName!, ValidDescription, false, ValidImageUrl, ValidPrice, 0, ValidCategories));
            
            Assert.Equal("Product name cannot be empty", exception.Message);
        }

        [Fact]
        public void GivenNameTooLong_ShouldThrowProductValidationException()
        {
            String longName = new String('a', 201);
            
            ProductValidationException exception = Assert.Throws<ProductValidationException>(() => 
                new Product(ValidId, longName, ValidDescription, false, ValidImageUrl, ValidPrice, 0, ValidCategories));
            
            Assert.Equal("Product name cannot be longer than 200 characters", exception.Message);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-0.01)]
        [InlineData(-1)]
        public void GivenInvalidPrice_ShouldThrowProductValidationException(Decimal invalidPrice)
        {
            ProductValidationException exception = Assert.Throws<ProductValidationException>(() => 
                new Product(ValidId, ValidName, ValidDescription, false, ValidImageUrl, invalidPrice, 0, ValidCategories));
            
            Assert.Equal("Price must be at least £0.01", exception.Message);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(-100)]
        public void GivenNegativeStock_ShouldThrowProductValidationException(Int32 invalidStock)
        {
            ProductValidationException exception = Assert.Throws<ProductValidationException>(() => 
                new Product(ValidId, ValidName, ValidDescription, false, ValidImageUrl, ValidPrice, invalidStock, ValidCategories));
            
            Assert.Equal("Stock cannot be negative", exception.Message);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        [InlineData("not-a-url")]
        public void GivenInvalidImageUrl_ShouldThrowProductValidationException(String? invalidUrl)
        {
            ProductValidationException exception = Assert.Throws<ProductValidationException>(() => 
                new Product(ValidId, ValidName, ValidDescription, false, invalidUrl!, ValidPrice, 0, ValidCategories));
            
            Assert.Contains("URL", exception.Message);
        }

        [Fact]
        public void GivenNullCategories_ShouldThrowProductValidationException()
        {
            ProductValidationException exception = Assert.Throws<ProductValidationException>(() => 
                new Product(ValidId, ValidName, ValidDescription, false, ValidImageUrl, ValidPrice, 0, null!));
            
            Assert.Equal("Product must have at least one category", exception.Message);
        }

        [Fact]
        public void GivenEmptyCategories_ShouldThrowProductValidationException()
        {
            ProductValidationException exception = Assert.Throws<ProductValidationException>(() => 
                new Product(ValidId, ValidName, ValidDescription,false, ValidImageUrl, ValidPrice, 0, []));
            
            Assert.Equal("Product must have at least one category", exception.Message);
        }

        [Fact]
        public void GivenCategoriesWithEmptyValue_ShouldThrowProductValidationException()
        {
            ProductValidationException exception = Assert.Throws<ProductValidationException>(() => 
                new Product(ValidId, ValidName, ValidDescription,false, ValidImageUrl, ValidPrice, 0, ["Valid", ""]));
            
            Assert.Equal("Categories cannot contain empty values", exception.Message);
        }
    }

    public class UpdatePrice
    {
        [Theory]
        [InlineData(10.00, 15.00)] // Normal increase
        [InlineData(10.00, 10.00)] // Same price
        [InlineData(0.01, 0.02)]   // Minimum valid increase
        public void GivenValidNewPrice_ShouldUpdatePrice(Decimal initialPrice, Decimal newPrice)
        {
            // Arrange
            Product product = new Product(ValidId, ValidName, ValidDescription,false, ValidImageUrl, initialPrice, 0, ValidCategories);

            // Act
            product.UpdatePrice(newPrice);

            // Assert
            Assert.Equal(newPrice, product.Price);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void GivenInvalidPrice_ShouldThrowProductValidationException(Decimal invalidPrice)
        {
            // Arrange
            Product product = CreateValidProduct();

            // Act & Assert
            ProductValidationException exception = Assert.Throws<ProductValidationException>(() => 
                product.UpdatePrice(invalidPrice));
            
            Assert.Equal("Price must be at least £0.01", exception.Message);
        }
    }

    public class StockManagement
    {
        [Theory]
        [InlineData(10, 5)]    // Normal removal
        [InlineData(10, 10)]   // Remove all stock
        public void GivenValidQuantity_RemoveStock_ShouldDecreaseStock(Int32 initialStock, Int32 quantityToRemove)
        {
            // Arrange
            Product product = CreateValidProduct(stock: initialStock);

            // Act
            product.RemoveStock(quantityToRemove);

            // Assert
            Assert.Equal(initialStock - quantityToRemove, product.Stock);
        }

        [Theory]
        [InlineData(10, 11)]   // More than available
        [InlineData(0, 1)]     // None available
        public void GivenInsufficientStock_RemoveStock_ShouldThrowInsufficientStockException(Int32 initialStock, Int32 quantityToRemove)
        {
            // Arrange
            Product product = CreateValidProduct(stock: initialStock);

            // Act & Assert
            Assert.Throws<InsufficientStockException>(() => 
                product.RemoveStock(quantityToRemove));
        }

        [Theory]
        [InlineData(0, 10)]    // Add to empty
        [InlineData(10, 5)]    // Normal addition
        public void GivenValidQuantity_AddStock_ShouldIncreaseStock(Int32 initialStock, Int32 quantityToAdd)
        {
            // Arrange
            Product product = CreateValidProduct(stock: initialStock);

            // Act
            product.AddStock(quantityToAdd);

            // Assert
            Assert.Equal(initialStock + quantityToAdd, product.Stock);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(-100)]
        public void GivenNegativeQuantity_AddStock_ShouldThrowProductValidationException(Int32 invalidQuantity)
        {
            // Arrange
            Product product = CreateValidProduct();

            // Act & Assert
            ProductValidationException exception = Assert.Throws<ProductValidationException>(() => 
                product.AddStock(invalidQuantity));
            
            Assert.Equal("Quantity cannot be negative", exception.Message);
        }

        [Fact]
        public void GivenQuantityThatWouldOverflow_AddStock_ShouldThrowInvalidOperationException()
        {
            // Arrange
            Product product = CreateValidProduct(stock: int.MaxValue - 5);

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => 
                product.AddStock(10));
        }
    }

    public class PropertyUpdates
    {
        [Fact]
        public void GivenValidName_Rename_ShouldUpdateName()
        {
            // Arrange
            Product product = CreateValidProduct();
            String newName = "New Product Name";

            // Act
            product.Rename(newName);

            // Assert
            Assert.Equal(newName, product.Name);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void GivenInvalidName_Rename_ShouldThrowProductValidationException(String? invalidName)
        {
            // Arrange
            Product product = CreateValidProduct();

            // Act & Assert
            ProductValidationException exception = Assert.Throws<ProductValidationException>(() => 
                product.Rename(invalidName!));
            
            Assert.Equal("Product name cannot be empty", exception.Message);
        }

        [Fact]
        public void GivenValidImageUrl_SetImage_ShouldUpdateImageUrl()
        {
            // Arrange
            Product product = CreateValidProduct();
            String newImageUrl = "https://example.com/new-image.jpg";

            // Act
            product.SetImage(newImageUrl);

            // Assert
            Assert.Equal(newImageUrl, product.ImageUrl);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        [InlineData("not-a-url")]
        public void GivenInvalidImageUrl_SetImage_ShouldThrowProductValidationException(String? invalidUrl)
        {
            // Arrange
            Product product = CreateValidProduct();

            // Act & Assert
            ProductValidationException exception = Assert.Throws<ProductValidationException>(() => 
                product.SetImage(invalidUrl!));
            
            Assert.Contains("URL", exception.Message);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void SetFeatured_ShouldUpdateFeaturedStatus(bool newStatus)
        {
            // Arrange
            Product product = CreateValidProduct();

            // Act
            product.SetFeatured(newStatus);

            // Assert
            Assert.Equal(newStatus, product.Featured);
        }
    }
}