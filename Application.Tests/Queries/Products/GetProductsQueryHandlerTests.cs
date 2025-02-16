using Application.DataTransferObjects;
using Application.Queries.Products;
using Application.Services;
using Domain.Entities;
using Domain.Repositories;
using Moq;
using Xunit;

namespace Application.Tests.Queries.Products;

public class GetProductsQueryHandlerTests
{
    private static readonly Guid ValidProductId = Guid.NewGuid();
    private static readonly Guid ValidCategoryId = Guid.NewGuid();
    private static readonly String ValidName = "Test Product";
    private static readonly String ValidDescription = "Test Description";
    private static readonly Decimal ValidPrice = 10.00m;
    private static readonly Int32 ValidStock = 5;
    private static readonly Boolean ValidFeatured = true;
    private static readonly String ValidImageUrl = "https://test.com/image.jpg";
    private static readonly List<String> ValidCategories = ["Category1"];

    private readonly Mock<IProductQueryService> _productRepositoryMock;
    private readonly GetProductsQueryHandler _handler;
    private readonly List<ProductSummary> _products;

    public GetProductsQueryHandlerTests()
    {
        _productRepositoryMock = new Mock<IProductQueryService>();
        _handler = new GetProductsQueryHandler(_productRepositoryMock.Object);
        _products = 
        [
            new ProductSummary
            {
                Id = ValidProductId,
                Name = ValidName,
                Categories = ValidCategories,
                Featured = ValidFeatured,
                ImageUrl = ValidImageUrl,
                Price = ValidPrice,
                Stock = ValidStock,
            }
        ];
    }

    public class HandleMethod : GetProductsQueryHandlerTests
    {
        [Fact]
        public async Task WithSearchQuery_ReturnsMatchingProducts()
        {
            // Arrange
            GetProductsQuery query = new GetProductsQuery { Search = ValidName };
            _productRepositoryMock
                .Setup(x => x.FindByPartialNameAsync(ValidName))
                .ReturnsAsync(_products);

            // Act
            GetProductsQueryResponse response = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(response);
            Assert.Single(response.Products);
            
            ProductSummary product = response.Products[0];
            Assert.Equal(ValidProductId, product.Id);
            Assert.Equal(ValidName, product.Name);
            Assert.Equal(ValidPrice, product.Price);
            Assert.Equal(ValidStock, product.Stock);
            Assert.Equal(ValidFeatured, product.Featured);
            Assert.Equal(ValidImageUrl, product.ImageUrl);
            Assert.Equal(ValidCategories, product.Categories);
        }

        [Fact]
        public async Task WithCategoryQuery_ReturnsMatchingProducts()
        {
            // Arrange
            GetProductsQuery query = new GetProductsQuery { Category = ValidCategoryId };
            _productRepositoryMock
                .Setup(x => x.FindByCategoryAsync(ValidCategoryId))
                .ReturnsAsync(_products);

            // Act
            GetProductsQueryResponse response = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(response);
            Assert.Single(response.Products);
            Assert.Equal(ValidProductId, response.Products[0].Id);
        }

        [Fact]
        public async Task WithFeaturedQuery_ReturnsMatchingProducts()
        {
            // Arrange
            GetProductsQuery query = new GetProductsQuery { Featured = true };
            _productRepositoryMock
                .Setup(x => x.FindFeaturedAsync())
                .ReturnsAsync(_products);

            // Act
            GetProductsQueryResponse response = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(response);
            Assert.Single(response.Products);
            Assert.Equal(ValidProductId, response.Products[0].Id);
        }

        [Fact]
        public async Task WithNoMatchingProducts_ReturnsEmptyList()
        {
            // Arrange
            GetProductsQuery query = new GetProductsQuery { Search = "NonExistent" };
            _productRepositoryMock
                .Setup(x => x.FindByPartialNameAsync("NonExistent"))
                .ReturnsAsync([]);

            // Act
            GetProductsQueryResponse response = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(response);
            Assert.Empty(response.Products);
        }

        [Fact]
        public async Task WithNoQueryParameters_ReturnsEmptyList()
        {
            // Arrange
            GetProductsQuery query = new GetProductsQuery();

            // Act
            GetProductsQueryResponse response = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(response);
            Assert.Empty(response.Products);
        }
    }
} 