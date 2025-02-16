using Domain.Exceptions.Customers;
using Domain.ValueObjects;
using Xunit;

namespace Domain.Tests.ValueObjects
{
    public class AddressTests
    {
        private static readonly String ValidFirstLine = "123 Main Street";
        private static readonly String ValidSecondLine = "Apartment 4B";
        private static readonly String ValidCity = "London";
        private static readonly String ValidCounty = "Greater London";
        private static readonly String ValidPostCode = "SW1A 1AA";

        private static Address CreateValidCustomerAddress(
            String? firstLine = null,
            String? secondLine = null,
            String? city = null,
            String? county = null,
            String? postCode = null)
        {
            return new Address(
                firstLine ?? ValidFirstLine,
                secondLine ?? ValidSecondLine,
                city ?? ValidCity,
                county ?? ValidCounty,
                postCode ?? ValidPostCode
            );
        }

        public class Constructor
        {
            [Fact]
            public void GivenValidParameters_ShouldCreateCustomerAddress()
            {
                // Act
                Address address = CreateValidCustomerAddress();

                // Assert
                Assert.Equal(ValidFirstLine, address.FirstLine);
                Assert.Equal(ValidSecondLine, address.SecondLine);
                Assert.Equal(ValidCity, address.City);
                Assert.Equal(ValidCounty, address.County);
                Assert.Equal(ValidPostCode.ToUpper(), address.PostCode);
            }

            [Theory]
            [InlineData("")]
            [InlineData(" ")]
            [InlineData(null)]
            public void GivenEmptyFirstLine_ShouldThrowAddressValidationException(String? invalidFirstLine)
            {
                AddressValidationException exception = Assert.Throws<AddressValidationException>(() =>
                    new Address(invalidFirstLine!, ValidSecondLine, ValidCity, ValidCounty, ValidPostCode));

                Assert.Equal("Address first line cannot be empty.", exception.Message);
            }

            [Fact]
            public void GivenFirstLineExceedingMaxLength_ShouldThrowAddressValidationException()
            {
                String tooLongFirstLine = new String('A', 101);

                AddressValidationException exception = Assert.Throws<AddressValidationException>(() =>
                    new Address(tooLongFirstLine, ValidSecondLine, ValidCity, ValidCounty, ValidPostCode));

                Assert.Equal("Address first line cannot exceed 100 characters.", exception.Message);
            }

            [Fact]
            public void GivenSecondLineExceedingMaxLength_ShouldThrowAddressValidationException()
            {
                String tooLongSecondLine = new String('A', 101);

                AddressValidationException exception = Assert.Throws<AddressValidationException>(() =>
                    new Address(ValidFirstLine, tooLongSecondLine, ValidCity, ValidCounty, ValidPostCode));

                Assert.Equal("Address second line cannot exceed 100 characters.", exception.Message);
            }

            [Theory]
            [InlineData("")]
            [InlineData(" ")]
            [InlineData(null)]
            public void GivenEmptyCity_ShouldThrowAddressValidationException(String? invalidCity)
            {
                AddressValidationException exception = Assert.Throws<AddressValidationException>(() =>
                    new Address(ValidFirstLine, ValidSecondLine, invalidCity!, ValidCounty, ValidPostCode));

                Assert.Equal("City cannot be empty.", exception.Message);
            }

            [Fact]
            public void GivenCityExceedingMaxLength_ShouldThrowAddressValidationException()
            {
                String tooLongCity = new String('A', 51);

                AddressValidationException exception = Assert.Throws<AddressValidationException>(() =>
                    new Address(ValidFirstLine, ValidSecondLine, tooLongCity, ValidCounty, ValidPostCode));

                Assert.Equal("City name cannot exceed 50 characters.", exception.Message);
            }

            [Theory]
            [InlineData("")]
            [InlineData(" ")]
            [InlineData(null)]
            public void GivenEmptyCounty_ShouldThrowAddressValidationException(String? invalidCounty)
            {
                AddressValidationException exception = Assert.Throws<AddressValidationException>(() =>
                    new Address(ValidFirstLine, ValidSecondLine, ValidCity, invalidCounty!, ValidPostCode));

                Assert.Equal("County cannot be empty.", exception.Message);
            }

            [Fact]
            public void GivenCountyExceedingMaxLength_ShouldThrowAddressValidationException()
            {
                String tooLongCounty = new String('A', 51);

                AddressValidationException exception = Assert.Throws<AddressValidationException>(() =>
                    new Address(ValidFirstLine, ValidSecondLine, ValidCity, tooLongCounty, ValidPostCode));

                Assert.Equal("County name cannot exceed 50 characters.", exception.Message);
            }

            [Theory]
            [InlineData("")]
            [InlineData(" ")]
            [InlineData(null)]
            public void GivenEmptyPostCode_ShouldThrowAddressValidationException(String? invalidPostCode)
            {
                AddressValidationException exception = Assert.Throws<AddressValidationException>(() =>
                    new Address(ValidFirstLine, ValidSecondLine, ValidCity, ValidCounty, invalidPostCode!));

                Assert.Equal("Postcode cannot be empty.", exception.Message);
            }

            [Theory]
            [InlineData("AB12")]
            [InlineData("ABCDEFGHIJ")] // Too long
            [InlineData("!@#$%^&*()")] // Special characters
            public void GivenInvalidPostCode_ShouldThrowAddressValidationException(String invalidPostCode)
            {
                AddressValidationException exception = Assert.Throws<AddressValidationException>(() =>
                    new Address(ValidFirstLine, ValidSecondLine, ValidCity, ValidCounty, invalidPostCode));

                Assert.Equal("Invalid postcode format.", exception.Message);
            }
        }

        public class Clone
        {
            [Fact]
            public void ShouldCreateNewInstanceWithSameValues()
            {
                // Arrange
                Address original = CreateValidCustomerAddress();

                // Act
                Address clone = original.Clone();

                // Assert
                Assert.NotSame(original, clone);
                Assert.Equal(original.FirstLine, clone.FirstLine);
                Assert.Equal(original.SecondLine, clone.SecondLine);
                Assert.Equal(original.City, clone.City);
                Assert.Equal(original.County, clone.County);
                Assert.Equal(original.PostCode, clone.PostCode);
            }
        }
    }
}