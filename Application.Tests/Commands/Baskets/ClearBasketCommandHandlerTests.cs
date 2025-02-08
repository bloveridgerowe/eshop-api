using Application.Commands.Baskets;
using Domain.Aggregates.Basket;
using Domain.Repositories;
using Moq;
using Xunit;

namespace Application.Tests.Commands.Baskets;

public class ClearBasketCommandHandlerTests
{
    private static readonly Guid ValidCustomerId = Guid.NewGuid();
    
    private readonly Mock<IBasketRepository> _basketRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly ClearBasketCommandHandler _handler;
    private readonly Basket _basket;

    public ClearBasketCommandHandlerTests()
    {
        _basketRepositoryMock = new Mock<IBasketRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _handler = new ClearBasketCommandHandler(_basketRepositoryMock.Object, _unitOfWorkMock.Object);
        _basket = new Basket(Guid.NewGuid(), ValidCustomerId);
        
        _basketRepositoryMock
            .Setup(x => x.FindByCustomerIdAsync(ValidCustomerId))
            .ReturnsAsync(_basket);
    }

    public class HandleMethod : ClearBasketCommandHandlerTests
    {
        [Fact]
        public async Task WithValidCommand_ClearsAllItems()
        {
            // Arrange
            _basket.AddItem(new BasketItem(Guid.NewGuid(), 1, 10.00m, "Test Product 1"));
            _basket.AddItem(new BasketItem(Guid.NewGuid(), 2, 20.00m, "Test Product 2"));
            ClearBasketCommand command = new()
            {
                CustomerId = ValidCustomerId
            };

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.Empty(_basket.Items);
            _basketRepositoryMock.Verify(x => x.SaveAsync(_basket), Times.Once);
            _unitOfWorkMock.Verify(x => x.CommitAsync(), Times.Once);
        }

        [Fact]
        public async Task WithEmptyBasket_StillProcessesSuccessfully()
        {
            // Arrange
            ClearBasketCommand command = new()
            {
                CustomerId = ValidCustomerId
            };

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.Empty(_basket.Items);
            _basketRepositoryMock.Verify(x => x.SaveAsync(_basket), Times.Once);
            _unitOfWorkMock.Verify(x => x.CommitAsync(), Times.Once);
        }

        [Fact]
        public async Task WithEmptyCustomerId_ThrowsArgumentException()
        {
            // Arrange
            ClearBasketCommand command = new()
            {
                CustomerId = Guid.Empty
            };

            // Act & Assert
            ArgumentException exception = await Assert.ThrowsAsync<ArgumentException>(
                () => _handler.Handle(command, CancellationToken.None));
            Assert.Equal("Customer ID cannot be empty (Parameter 'CustomerId')", exception.Message);
            _basketRepositoryMock.Verify(x => x.FindByCustomerIdAsync(It.IsAny<Guid>()), Times.Never);
            _basketRepositoryMock.Verify(x => x.SaveAsync(It.IsAny<Basket>()), Times.Never);
            _unitOfWorkMock.Verify(x => x.CommitAsync(), Times.Never);
        }
    }
}