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
    private static readonly String ValidDescription = "Test Description";
    private static readonly Int32 ValidStock = 5;
    private static readonly Boolean ValidFeatured = true;
    private static readonly String ValidImageUrl = "https://test.com/image.jpg";
    private static readonly List<String> ValidCategories = ["Category1", "Category2"];

    [Fact]
    public void ToSummaryQueryModel_MapsAllPropertiesCorrectly()
    {
        // Arrange
        Product product = new Product(
            ValidId,
            ValidName,
            ValidDescription,
            ValidFeatured,
            ValidImageUrl,
            ValidPrice,
            ValidStock,
            ValidCategories
        );

        // Act
        ProductSummary result = product.ToSummaryQueryModel();

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