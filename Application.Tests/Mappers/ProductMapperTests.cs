using Application.DataTransferObjects;
using Application.Mappers;
using Domain.Entities;
using Xunit;

namespace Application.Tests.Mappers;

public class ProductMapperTests
{
    private static readonly Guid ValidId = Guid.NewGuid();
    private static readonly String ValidName = "Test Product";
    private static readonly Decimal ValidPrice = 10.00m;
    private static readonly Int32 ValidStock = 5;
    private static readonly bool ValidFeatured = true;
    private static readonly String ValidImageUrl = "https://test.com/image.jpg";
    private static readonly List<String> ValidCategories = ["Category1", "Category2"];

    [Fact]
    public void ToCommandQueryModel_MapsAllPropertiesCorrectly()
    {
        // Arrange
        Product product = new Product(
            ValidId,
            ValidName,
            ValidFeatured,
            ValidImageUrl,
            ValidPrice,
            ValidStock,
            ValidCategories
        );

        // Act
        ProductDetails result = product.ToCommandQueryModel();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(ValidId, result.Id);
        Assert.Equal(ValidName, result.Name);
        Assert.Equal(ValidPrice, result.Price);
        Assert.Equal(ValidStock, result.Stock);
        Assert.Equal(ValidFeatured, result.Featured);
        Assert.Equal(ValidImageUrl, result.ImageUrl);
        Assert.Equal(ValidCategories, result.Categories);
    }
} 