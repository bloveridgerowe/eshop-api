using Application.Commands.Customers;
using Domain.Entities;
using Domain.Repositories;
using Moq;
using Xunit;

namespace Application.Tests.Commands.Customers
{
    public class UpdateCustomerDetailsCommandHandlerTests
    {
        private static readonly Guid ValidCustomerId = Guid.NewGuid();
        private static readonly String ValidEmail = "test@example.com";
        private static readonly String ValidName = "John Doe";
        private static readonly UpdateAddressDetails ValidAddress = new UpdateAddressDetails
        {
            FirstLine = "123 Main Street",
            SecondLine = "Apartment 4B",
            City = "London",
            County = "Greater London",
            PostCode = "SW1A 1AA"
        };

        private static readonly UpdateCardDetails ValidCardDetails = new UpdateCardDetails
        {
            CardNumber = "1234567890123456",
            ExpiryDate = "12/25",
            Cvv = "123"
        };

        private readonly Mock<ICustomerRepository> _customerRepositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly UpdateCustomerDetailsCommandHandler _handler;
        private readonly Customer _customer;

        public UpdateCustomerDetailsCommandHandlerTests()
        {
            _customerRepositoryMock = new Mock<ICustomerRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            
            _handler = new UpdateCustomerDetailsCommandHandler(_customerRepositoryMock.Object, _unitOfWorkMock.Object);
            _customer = new Customer(ValidCustomerId, ValidEmail);

            _customerRepositoryMock
                .Setup(x => x.FindByIdAsync(ValidCustomerId))
                .ReturnsAsync(_customer);
        }

        public class HandleMethod : UpdateCustomerDetailsCommandHandlerTests
        {
            [Fact]
            public async Task WithValidCommand_UpdatesAllFields()
            {
                // Arrange
                UpdateCustomerDetailsCommand command = new()
                {
                    CustomerId = ValidCustomerId,
                    Name = ValidName,
                    Address = ValidAddress,
                    CardDetails = ValidCardDetails
                };

                // Act
                await _handler.Handle(command, CancellationToken.None);

                // Assert
                // TODO: Fix
                Assert.Equal(ValidName, _customer.Name);
                Assert.Equal(ValidAddress.FirstLine, _customer.Address!.FirstLine);
                Assert.Equal(ValidAddress.SecondLine, _customer.Address.SecondLine);
                Assert.Equal(ValidAddress.City, _customer.Address.City);
                Assert.Equal(ValidAddress.County, _customer.Address.County);
                Assert.Equal(ValidAddress.PostCode, _customer.Address.PostCode);
                Assert.Equal(ValidCardDetails.CardNumber, _customer.CardDetails!.CardNumber);
                Assert.Equal(ValidCardDetails.Cvv, _customer.CardDetails!.Cvv);
                Assert.Equal(ValidCardDetails.ExpiryDate, _customer.CardDetails.ExpiryDate);
                _customerRepositoryMock.Verify(x => x.SaveAsync(_customer), Times.Once);
            }

            [Fact]
            public async Task WithOnlyName_UpdatesOnlyName()
            {
                // Arrange
                UpdateCustomerDetailsCommand command = new()
                {
                    CustomerId = ValidCustomerId,
                    Name = ValidName
                };

                // Act
                await _handler.Handle(command, CancellationToken.None);

                // Assert
                Assert.Equal(ValidName, _customer.Name);
                Assert.Null(_customer.Address);
                Assert.Null(_customer.CardDetails);
                _customerRepositoryMock.Verify(x => x.SaveAsync(_customer), Times.Once);
            }

            [Fact]
            public async Task WithOnlyAddress_UpdatesOnlyAddress()
            {
                // Arrange
                UpdateCustomerDetailsCommand command = new()
                {
                    CustomerId = ValidCustomerId,
                    Address = ValidAddress
                };

                // Act
                await _handler.Handle(command, CancellationToken.None);

                // Assert
                Assert.Null(_customer.Name);
                Assert.Equal(ValidAddress.FirstLine, _customer.Address!.FirstLine);
                Assert.Equal(ValidAddress.SecondLine, _customer.Address.SecondLine);
                Assert.Equal(ValidAddress.City, _customer.Address.City);
                Assert.Equal(ValidAddress.County, _customer.Address.County);
                Assert.Equal(ValidAddress.PostCode, _customer.Address.PostCode);                Assert.Null(_customer.CardDetails);
                _customerRepositoryMock.Verify(x => x.SaveAsync(_customer), Times.Once);
            }

            [Fact]
            public async Task WithOnlyCardDetails_UpdatesOnlyCardDetails()
            {
                // Arrange
                UpdateCustomerDetailsCommand command = new()
                {
                    CustomerId = ValidCustomerId,
                    CardDetails = ValidCardDetails
                };

                // Act
                await _handler.Handle(command, CancellationToken.None);

                // Assert
                Assert.Null(_customer.Name);
                Assert.Null(_customer.Address);
                Assert.Equal(ValidCardDetails.CardNumber, _customer.CardDetails!.CardNumber);
                Assert.Equal(ValidCardDetails.Cvv, _customer.CardDetails!.Cvv);
                Assert.Equal(ValidCardDetails.ExpiryDate, _customer.CardDetails.ExpiryDate);
                _customerRepositoryMock.Verify(x => x.SaveAsync(_customer), Times.Once);
            }

            [Fact]
            public async Task WithEmptyCustomerId_ThrowsArgumentException()
            {
                // Arrange
                UpdateCustomerDetailsCommand command = new()
                {
                    CustomerId = Guid.Empty,
                    Name = ValidName
                };

                // Act & Assert
                ArgumentException exception = await Assert.ThrowsAsync<ArgumentException>(
                    () => _handler.Handle(command, CancellationToken.None));
                Assert.Equal("Customer ID cannot be empty (Parameter 'CustomerId')", exception.Message);
                _customerRepositoryMock.Verify(x => x.FindByIdAsync(It.IsAny<Guid>()), Times.Never);
                _customerRepositoryMock.Verify(x => x.SaveAsync(It.IsAny<Customer>()), Times.Never);
            }

            [Fact]
            public async Task WithCustomerNotFound_ThrowsArgumentException()
            {
                // Arrange
                UpdateCustomerDetailsCommand command = new()
                {
                    CustomerId = Guid.NewGuid(),
                    Name = ValidName
                };

                _customerRepositoryMock
                    .Setup(x => x.FindByIdAsync(command.CustomerId))
                    .ReturnsAsync((Customer?)null);

                // Act & Assert
                ArgumentException exception = await Assert.ThrowsAsync<ArgumentException>(
                    () => _handler.Handle(command, CancellationToken.None));
                Assert.Equal($"Customer with ID {command.CustomerId} not found. (Parameter 'CustomerId')", exception.Message);
                _customerRepositoryMock.Verify(x => x.SaveAsync(It.IsAny<Customer>()), Times.Never);
            }

            [Theory]
            [InlineData("")]
            [InlineData(" ")]
            public async Task WithEmptyName_ThrowsArgumentException(String emptyName)
            {
                // Arrange
                UpdateCustomerDetailsCommand command = new()
                {
                    CustomerId = ValidCustomerId,
                    Name = emptyName
                };

                // Act & Assert
                ArgumentException exception = await Assert.ThrowsAsync<ArgumentException>(
                    () => _handler.Handle(command, CancellationToken.None));
                Assert.Equal("Name cannot be empty if provided (Parameter 'Name')", exception.Message);
                _customerRepositoryMock.Verify(x => x.FindByIdAsync(It.IsAny<Guid>()), Times.Never);
                _customerRepositoryMock.Verify(x => x.SaveAsync(It.IsAny<Customer>()), Times.Never);
            }
        }
    }
}