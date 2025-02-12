using Application.Queries.Customer;
using Domain.Entities;
using Domain.Repositories;
using Moq;
using Domain.ValueObjects;
using Xunit;

namespace Application.Tests.Queries.Customers
{
    public class GetCustomerDetailsQueryHandlerTests
    {
        private static readonly Guid ValidCustomerId = Guid.NewGuid();
        private static readonly String ValidEmail = "test@example.com";
        private static readonly String ValidName = "John Doe";
        private static readonly Address ValidAddress = new(
            "123 Main Street",
            "Apartment 4B",
            "London",
            "Greater London",
            "SW1A 1AA"
        );
        private static readonly CardDetails ValidCardDetails = new("1234567890123456", "12/25", "123");

        private readonly Mock<ICustomerRepository> _customerRepositoryMock;
        private readonly GetCustomerDetailsQueryHandler _handler;
        private readonly Customer _customer;

        public GetCustomerDetailsQueryHandlerTests()
        {
            _customerRepositoryMock = new Mock<ICustomerRepository>();
            _handler = new GetCustomerDetailsQueryHandler(_customerRepositoryMock.Object);

            _customer = new Customer(ValidCustomerId, ValidEmail, ValidName, ValidAddress, ValidCardDetails);
        }

        public class HandleMethod : GetCustomerDetailsQueryHandlerTests
        {
            [Fact]
            public async Task WithExistingCustomer_ReturnsCorrectResponse()
            {
                // Arrange
                GetCustomerDetailsQuery query = new GetCustomerDetailsQuery { CustomerId = ValidCustomerId };
                _customerRepositoryMock
                    .Setup(x => x.FindByIdAsync(ValidCustomerId))
                    .ReturnsAsync(_customer);

                // Act
                GetCustomerDetailsQueryResponse response = await _handler.Handle(query, CancellationToken.None);

                // Assert
                Assert.NotNull(response.CustomerDetails);
                Assert.Equal(ValidName, response.CustomerDetails.Name);
                Assert.Equal(ValidEmail, response.CustomerDetails.Email);
                Assert.Equal(ValidAddress.FirstLine, response.CustomerDetails.Address?.FirstLine);
                Assert.Equal(ValidAddress.SecondLine, response.CustomerDetails.Address?.SecondLine);
                Assert.Equal(ValidAddress.City, response.CustomerDetails.Address?.City);
                Assert.Equal(ValidAddress.County, response.CustomerDetails.Address?.County);
                Assert.Equal(ValidAddress.PostCode, response.CustomerDetails.Address?.PostCode);
                
                Assert.NotNull(response.CustomerDetails.CardDetails);
                Assert.Equal(ValidCardDetails.MaskedCardNumber, response.CustomerDetails.CardDetails.MaskedCardNumber);
                Assert.Equal(ValidCardDetails.ExpiryDate, response.CustomerDetails.CardDetails.ExpiryDate);
                Assert.Equal(ValidCardDetails.MaskedCvv, response.CustomerDetails.CardDetails.MaskedCvv);
            }

            [Fact]
            public async Task WithNonExistingCustomer_ReturnsNull()
            {
                // Arrange
                GetCustomerDetailsQuery query = new GetCustomerDetailsQuery { CustomerId = ValidCustomerId };
                _customerRepositoryMock
                    .Setup(x => x.FindByIdAsync(ValidCustomerId))
                    .ReturnsAsync((Customer?)null);

                // Act
                GetCustomerDetailsQueryResponse response = await _handler.Handle(query, CancellationToken.None);

                // Assert
                Assert.Null(response.CustomerDetails);
            }

            [Fact]
            public async Task WithCustomerHavingOnlyRequiredFields_ReturnsCorrectResponse()
            {
                // Arrange
                Customer basicCustomer = new Customer(ValidCustomerId, ValidEmail);
                GetCustomerDetailsQuery query = new GetCustomerDetailsQuery { CustomerId = ValidCustomerId };
                _customerRepositoryMock
                    .Setup(x => x.FindByIdAsync(ValidCustomerId))
                    .ReturnsAsync(basicCustomer);

                // Act
                GetCustomerDetailsQueryResponse response = await _handler.Handle(query, CancellationToken.None);

                // Assert
                Assert.NotNull(response.CustomerDetails);
                Assert.Equal(ValidEmail, response.CustomerDetails.Email);
                Assert.Null(response.CustomerDetails.Name);
                Assert.Null(response.CustomerDetails.Address);
                Assert.Null(response.CustomerDetails.CardDetails);
            }
        }
    }
}