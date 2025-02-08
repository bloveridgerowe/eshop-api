using Application.DataTransferObjects;
using Application.Queries.Baskets;
using Domain.Aggregates.Basket;
using Domain.Repositories;
using Moq;
using Xunit;

namespace Application.Tests.Queries.Baskets;

public class GetBasketDetailsQueryHandlerTests
{
    private static readonly Guid ValidCustomerId = Guid.NewGuid();
    private static readonly Guid ValidBasketId = Guid.NewGuid();
    private static readonly Guid ValidProductId = Guid.NewGuid();
    private static readonly String ValidProductName = "Test Product";
    private static readonly Decimal ValidPrice = 10.00m;
    private static readonly Int32 ValidQuantity = 1;
    private static readonly DateTime ValidCreatedAt = new(2024, 1, 1, 10, 0, 0, DateTimeKind.Utc);
    private static readonly DateTime ValidUpdatedAt = new(2024, 1, 1, 11, 0, 0, DateTimeKind.Utc);

    private readonly Mock<IBasketRepository> _basketRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly GetBasketDetailsQueryHandler _handler;
    private readonly Basket _basket;

    public GetBasketDetailsQueryHandlerTests()
    {
        _basketRepositoryMock = new Mock<IBasketRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        
        _handler = new GetBasketDetailsQueryHandler(_basketRepositoryMock.Object, _unitOfWorkMock.Object);

        _basket = new Basket(ValidBasketId, ValidCustomerId);
        _basket.AddItem(new BasketItem(ValidProductId, ValidQuantity, ValidPrice, ValidProductName));
        
        typeof(Basket).GetProperty(nameof(Basket.CreatedAt))!.SetValue(_basket, ValidCreatedAt);
        typeof(Basket).GetProperty(nameof(Basket.UpdatedAt))!.SetValue(_basket, ValidUpdatedAt);
    }

    public class HandleMethod : GetBasketDetailsQueryHandlerTests
    {
        [Fact]
        public async Task WithExistingBasket_ReturnsCorrectResponse()
        {
            // Arrange
            GetBasketDetailsQuery query = new GetBasketDetailsQuery { CustomerId = ValidCustomerId };
            _basketRepositoryMock
                .Setup(x => x.FindByCustomerIdAsync(ValidCustomerId))
                .ReturnsAsync(_basket);

            // Act
            GetBasketDetailsQueryResponse response = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(response.BasketDetails);
            Assert.Equal(ValidBasketId, response.BasketDetails.Id);
            Assert.Equal(ValidCustomerId, response.BasketDetails.CustomerId);
            Assert.Equal(ValidCreatedAt, response.BasketDetails.CreatedAt);
            Assert.Equal(ValidUpdatedAt, response.BasketDetails.UpdatedAt);
            Assert.Equal(ValidPrice * ValidQuantity, response.BasketDetails.TotalPrice);
            
            Assert.Single(response.BasketDetails.Items);
            BasketItemDetails item = response.BasketDetails.Items[0];
            Assert.Equal(ValidProductId, item.ProductId);
            Assert.Equal(ValidQuantity, item.Quantity);
            Assert.Equal(ValidPrice, item.Price);
            Assert.Equal(ValidPrice * ValidQuantity, item.TotalPrice);
        }

        [Fact]
        public async Task WithEmptyBasket_ReturnsBasketWithNoItems()
        {
            // Arrange
            Basket emptyBasket = new Basket(ValidBasketId, ValidCustomerId);
            // Set timestamps after creation to avoid UpdatedAt being set to UtcNow
            typeof(Basket)
                .GetProperty(nameof(Basket.CreatedAt))!
                .SetValue(emptyBasket, ValidCreatedAt);
            typeof(Basket)
                .GetProperty(nameof(Basket.UpdatedAt))!
                .SetValue(emptyBasket, ValidUpdatedAt);
                
            GetBasketDetailsQuery query = new GetBasketDetailsQuery { CustomerId = ValidCustomerId };
            _basketRepositoryMock
                .Setup(x => x.FindByCustomerIdAsync(ValidCustomerId))
                .ReturnsAsync(emptyBasket);

            // Act
            GetBasketDetailsQueryResponse response = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(response.BasketDetails);
            Assert.Equal(ValidBasketId, response.BasketDetails.Id);
            Assert.Equal(ValidCustomerId, response.BasketDetails.CustomerId);
            Assert.Empty(response.BasketDetails.Items);
            Assert.Equal(0, response.BasketDetails.TotalPrice);
            Assert.Equal(ValidCreatedAt, response.BasketDetails.CreatedAt);
            Assert.Equal(ValidUpdatedAt, response.BasketDetails.UpdatedAt);
        }
    }
} 