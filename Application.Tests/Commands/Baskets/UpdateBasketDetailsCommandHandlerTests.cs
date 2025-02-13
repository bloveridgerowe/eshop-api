using Application.Commands.Baskets;
using Domain.Aggregates.Basket;
using Domain.Entities;
using Domain.Exceptions.Products;
using Domain.Repositories;
using Moq;
using Xunit;

namespace Application.Tests.Commands.Baskets;

public class UpdateBasketDetailsCommandHandlerTests
{
    private static readonly Guid ValidCustomerId = Guid.NewGuid();
    private static readonly Guid ValidBasketId = Guid.NewGuid();
    private static readonly Guid ValidProductId = Guid.NewGuid();
    private static readonly String ValidProductName = "Test Product";
    private static readonly String ValidProductDescription = "Test Description";
    private static readonly Product ValidProduct = new(
        Guid.NewGuid(),
        ValidProductName,
        ValidProductDescription,
        false,
        "https://test.image.com/",
        9.99M,
        10,
        ["Test Category"]);
    
    private static readonly UpdateBasketItemDetails ValidItem = new()
    {
        ProductId = ValidProductId,
        Quantity = 1,
    };
    
    private readonly Mock<IBasketRepository> _basketRepositoryMock;
    private readonly Mock<IProductRepository> _productRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly UpdateBasketDetailsCommandHandler _handler;
    private readonly Basket _basket;

    public UpdateBasketDetailsCommandHandlerTests()
    {
        _basketRepositoryMock = new Mock<IBasketRepository>();
        _productRepositoryMock = new Mock<IProductRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        
        _productRepositoryMock.Setup(p => p.FindByIdAsync(ValidProductId)).ReturnsAsync(ValidProduct);
        _handler = new UpdateBasketDetailsCommandHandler(_basketRepositoryMock.Object, _productRepositoryMock.Object, _unitOfWorkMock.Object);
        _basket = new Basket(ValidBasketId, ValidCustomerId);
        
        _basketRepositoryMock
            .Setup(x => x.FindByCustomerIdAsync(ValidCustomerId))
            .ReturnsAsync(_basket);
    }

    public class HandleMethod : UpdateBasketDetailsCommandHandlerTests
    {
        [Fact]
        public async Task WithValidCommand_AddsNewItem()
        {
            // Arrange
            UpdateBasketDetailsCommand command = new()
            {
                CustomerId = ValidCustomerId,
                Items = [ValidItem]
            };

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.Single(_basket.Items);
            BasketItem addedItem = _basket.Items[0];
            Assert.Equal(ValidProductId, addedItem.ProductId);
            Assert.Equal(ValidItem.Quantity, addedItem.Quantity);
            _basketRepositoryMock.Verify(x => x.SaveAsync(_basket), Times.Once);
            _unitOfWorkMock.Verify(x => x.CommitAsync(), Times.Once);
        }

        [Fact]
        public async Task WithNoExistingBasket_CreatesNewBasketAndAddsItem()
        {
            // Arrange
            _basketRepositoryMock
                .Setup(x => x.FindByCustomerIdAsync(ValidCustomerId))
                .ReturnsAsync((Basket?)null);

            UpdateBasketDetailsCommand command = new()
            {
                CustomerId = ValidCustomerId,
                Items = [ValidItem]
            };

            Basket? savedBasket = null;
            _basketRepositoryMock
                .Setup(x => x.SaveAsync(It.IsAny<Basket>()))
                .Callback<Basket>(basket => savedBasket = basket);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(savedBasket);
            Assert.Equal(ValidCustomerId, savedBasket!.CustomerId);
            Assert.Single(savedBasket.Items);
            BasketItem addedItem = savedBasket.Items[0];
            Assert.Equal(ValidProductId, addedItem.ProductId);
            Assert.Equal(ValidItem.Quantity, addedItem.Quantity);
            _basketRepositoryMock.Verify(x => x.SaveAsync(It.IsAny<Basket>()), Times.Once);
            _unitOfWorkMock.Verify(x => x.CommitAsync(), Times.Exactly(2)); // Once for creation, once for update
        }

        [Fact]
        public async Task WithExistingItem_UpdatesQuantityAndPrice()
        {
            // Arrange
            _basket.AddItem(new BasketItem(ValidProductId, 1, 10.00m, ValidProductName));
            UpdateBasketDetailsCommand command = new()
            {
                CustomerId = ValidCustomerId,
                Items = [new UpdateBasketItemDetails
                {
                    ProductId = ValidProductId,
                    Quantity = 2,
                }]
            };

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.Single(_basket.Items);
            BasketItem updatedItem = _basket.Items[0];
            Assert.Equal(ValidProductId, updatedItem.ProductId);
            Assert.Equal(2, updatedItem.Quantity);
            _basketRepositoryMock.Verify(x => x.SaveAsync(_basket), Times.Once);
            _unitOfWorkMock.Verify(x => x.CommitAsync(), Times.Once);
        }

        [Fact]
        public async Task WithZeroQuantity_RemovesItem()
        {
            // Arrange
            _basket.AddItem(new BasketItem(ValidProductId, 1, 10.00m, ValidProductName));
            UpdateBasketDetailsCommand command = new()
            {
                CustomerId = ValidCustomerId,
                Items = [new UpdateBasketItemDetails
                {
                    ProductId = ValidProductId,
                    Quantity = 0,
                }]
            };

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.Empty(_basket.Items);
            _basketRepositoryMock.Verify(x => x.SaveAsync(_basket), Times.Once);
            _unitOfWorkMock.Verify(x => x.CommitAsync(), Times.Once);
        }
        
        [Fact]
        public async Task WithProductNotFound_ThrowsProductNotFoundException()
        {
            // Arrange
            Guid nonExistentProductId = Guid.NewGuid();
            UpdateBasketDetailsCommand command = new()
            {
                CustomerId = ValidCustomerId,
                Items = [new UpdateBasketItemDetails
                {
                    ProductId = nonExistentProductId,
                    Quantity = 1,
                }]
            };
            
            // Act & Assert
            ProductNotFoundException exception = await Assert.ThrowsAsync<ProductNotFoundException>(() => _handler.Handle(command, CancellationToken.None));
            Assert.Equal($"Product with ID {nonExistentProductId} was not found", exception.Message);
            _basketRepositoryMock.Verify(x => x.SaveAsync(It.IsAny<Basket>()), Times.Never);
            _unitOfWorkMock.Verify(x => x.CommitAsync(), Times.Never);
        }

        [Fact]
        public async Task WithEmptyCustomerId_ThrowsArgumentException()
        {
            // Arrange
            UpdateBasketDetailsCommand command = new()
            {
                CustomerId = Guid.Empty,
                Items = [ValidItem]
            };

            // Act & Assert
            ArgumentException exception = await Assert.ThrowsAsync<ArgumentException>(
                () => _handler.Handle(command, CancellationToken.None));
            Assert.Equal("Basket ID cannot be empty (Parameter 'CustomerId')", exception.Message);
            _basketRepositoryMock.Verify(x => x.FindByCustomerIdAsync(It.IsAny<Guid>()), Times.Never);
            _basketRepositoryMock.Verify(x => x.SaveAsync(It.IsAny<Basket>()), Times.Never);
            _unitOfWorkMock.Verify(x => x.CommitAsync(), Times.Never);
        }

        [Fact]
        public async Task WithEmptyProductId_ThrowsArgumentException()
        {
            // Arrange
            UpdateBasketDetailsCommand command = new()
            {
                CustomerId = ValidCustomerId,
                Items = [new UpdateBasketItemDetails
                {
                    ProductId = Guid.Empty,
                    Quantity = 1,
                }]
            };

            // Act & Assert
            ArgumentException exception = await Assert.ThrowsAsync<ArgumentException>(
                () => _handler.Handle(command, CancellationToken.None));
            Assert.Equal("Product ID cannot be empty (Parameter 'Items')", exception.Message);
            _basketRepositoryMock.Verify(x => x.FindByCustomerIdAsync(It.IsAny<Guid>()), Times.Never);
            _basketRepositoryMock.Verify(x => x.SaveAsync(It.IsAny<Basket>()), Times.Never);
            _unitOfWorkMock.Verify(x => x.CommitAsync(), Times.Never);
        }

        [Fact]
        public async Task WithNegativeQuantity_ThrowsArgumentException()
        {
            // Arrange
            UpdateBasketDetailsCommand command = new()
            {
                CustomerId = ValidCustomerId,
                Items = [new UpdateBasketItemDetails
                {
                    ProductId = ValidProductId,
                    Quantity = -1,
                }]
            };

            // Act & Assert
            ArgumentException exception = await Assert.ThrowsAsync<ArgumentException>(
                () => _handler.Handle(command, CancellationToken.None));
            Assert.Equal("Quantity cannot be negative (Parameter 'Items')", exception.Message);
            _basketRepositoryMock.Verify(x => x.FindByCustomerIdAsync(It.IsAny<Guid>()), Times.Never);
            _basketRepositoryMock.Verify(x => x.SaveAsync(It.IsAny<Basket>()), Times.Never);
            _unitOfWorkMock.Verify(x => x.CommitAsync(), Times.Never);
        }
    }
} 