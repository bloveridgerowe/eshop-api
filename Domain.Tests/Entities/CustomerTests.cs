using Domain.Entities;
using Domain.ValueObjects;
using Xunit;

namespace Domain.Tests.Entities
{
    public class CustomerTests
    {
        private static readonly Guid ValidId = Guid.NewGuid();
        private static readonly String ValidEmail = "test@example.com";
        private static readonly String ValidName = "John Smith";
        private static readonly Address ValidAddress = new(
            "123 Main Street",
            "Apartment 4B",
            "London",
            "Greater London",
            "SW1A 1AA"
        );
        private static readonly CardDetails ValidCardDetails = new(
            "4111111111111111",
            $"{DateTime.UtcNow.AddMonths(1):MM/yy}",
            "123"
        );

        private static Customer CreateValidCustomer(
            Guid? id = null,
            String? email = null,
            String? name = null,
            Address? address = null,
            CardDetails? cardDetails = null)
        {
            return new Customer(
                id ?? ValidId,
                email ?? ValidEmail,
                name,
                address,
                cardDetails
            );
        }

        public class Constructor
        {
            [Fact]
            public void GivenValidParameters_ShouldCreateCustomer()
            {
                // Act
                Customer customer = CreateValidCustomer(
                    name: ValidName,
                    address: ValidAddress,
                    cardDetails: ValidCardDetails
                );

                // Assert
                Assert.Equal(ValidId, customer.Id);
                Assert.Equal(ValidEmail, customer.Email);
                Assert.Equal(ValidName, customer.Name);
                Assert.Equal(ValidAddress, customer.Address);
                Assert.NotNull(customer.CardDetails);
                Assert.NotSame(ValidCardDetails, customer.CardDetails); // Defensive copy
            }

            [Fact]
            public void GivenMinimalParameters_ShouldCreateCustomer()
            {
                // Act
                Customer customer = CreateValidCustomer();

                // Assert
                Assert.Equal(ValidId, customer.Id);
                Assert.Equal(ValidEmail, customer.Email);
                Assert.Null(customer.Name);
                Assert.Null(customer.Address);
                Assert.Null(customer.CardDetails);
            }
        }

        public class UpdateAddress
        {
            [Fact]
            public void GivenValidAddress_ShouldUpdateAddress()
            {
                // Arrange
                Customer customer = CreateValidCustomer();
                Address newAddress = new(
                    "456 Other Street",
                    "Suite 7",
                    "Manchester",
                    "Greater Manchester",
                    "M1 1AA"
                );

                // Act
                customer.UpdateAddress(newAddress);

                // Assert
                Assert.Equal(newAddress, customer.Address);
            }

            [Fact]
            public void GivenNull_ShouldClearAddress()
            {
                // Arrange
                Customer customer = CreateValidCustomer(address: ValidAddress);

                // Act
                customer.UpdateAddress(null);

                // Assert
                Assert.Null(customer.Address);
            }
        }
    }
}