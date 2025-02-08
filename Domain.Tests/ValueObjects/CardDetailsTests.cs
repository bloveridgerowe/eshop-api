using Domain.Exceptions.Customers;
using Domain.ValueObjects;
using Xunit;

namespace Domain.Tests.ValueObjects;

public class CardDetailsTests
{
    private static readonly String ValidCardNumber = "4111111111111111";
    private static readonly String ValidExpiryDate = $"{DateTime.UtcNow.AddMonths(1):MM/yy}";
    private static readonly String ValidCvv = "123";
    
    private static CardDetails CreateValidCardDetails(
        String? cardNumber = null,
        String? expiryDate = null,
        String? cvv = null)
    {
        return new CardDetails(
            cardNumber ?? ValidCardNumber,
            expiryDate ?? ValidExpiryDate,
            cvv ?? ValidCvv
        );
    }

    public class Constructor
    {
        [Fact]
        public void GivenValidParameters_ShouldCreateCardDetails()
        {
            // Act
            CardDetails cardDetails = CreateValidCardDetails();

            // Assert
            Assert.Equal($"************{ValidCardNumber[^4..]}", cardDetails.MaskedCardNumber);
            Assert.Equal(ValidExpiryDate, cardDetails.ExpiryDate);
            Assert.Equal(new String('*', ValidCvv.Length), cardDetails.MaskedCvv);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void GivenEmptyCardNumber_ShouldThrowCardValidationException(String? invalidCardNumber)
        {
            CardValidationException exception = Assert.Throws<CardValidationException>(() => 
                new CardDetails(invalidCardNumber!, ValidExpiryDate, ValidCvv));
            
            Assert.Equal("Card number cannot be empty", exception.Message);
        }

        [Theory]
        [InlineData("411111111111111")] // 15 digits
        [InlineData("41111111111111111")] // 17 digits
        [InlineData("4111-1111-1111-1111")] // With dashes
        [InlineData("4111 1111 1111 1111")] // With spaces
        [InlineData("abcdefghijklmnop")] // Letters
        public void GivenInvalidCardNumber_ShouldThrowCardValidationException(String invalidCardNumber)
        {
            CardValidationException exception = Assert.Throws<CardValidationException>(() => 
                new CardDetails(invalidCardNumber, ValidExpiryDate, ValidCvv));
            
            Assert.Equal("Card number must be 16 digits", exception.Message);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void GivenEmptyExpiryDate_ShouldThrowCardValidationException(String? invalidExpiryDate)
        {
            CardValidationException exception = Assert.Throws<CardValidationException>(() => 
                new CardDetails(ValidCardNumber, invalidExpiryDate!, ValidCvv));
            
            Assert.Equal("Expiry date cannot be empty", exception.Message);
        }

        [Theory]
        [InlineData("13/23")] // Invalid month
        [InlineData("00/23")] // Invalid month
        [InlineData("1/23")] // Single digit month
        [InlineData("01/2023")] // Full year
        [InlineData("01-23")] // Wrong separator
        [InlineData("0123")] // No separator
        [InlineData("aa/bb")] // Letters
        public void GivenInvalidExpiryDateFormat_ShouldThrowCardValidationException(String invalidExpiryDate)
        {
            CardValidationException exception = Assert.Throws<CardValidationException>(() => 
                new CardDetails(ValidCardNumber, invalidExpiryDate, ValidCvv));
            
            Assert.Equal("Expiry date must be in MM/YY format", exception.Message);
        }

        [Fact]
        public void GivenExpiredCard_ShouldThrowCardValidationException()
        {
            String expiredDate = DateTime.UtcNow.AddMonths(-1).ToString("MM/yy");
            
            CardValidationException exception = Assert.Throws<CardValidationException>(() => 
                new CardDetails(ValidCardNumber, expiredDate, ValidCvv));
            
            Assert.Equal("Card has expired", exception.Message);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void GivenEmptyCvv_ShouldThrowCardValidationException(String? invalidCvv)
        {
            CardValidationException exception = Assert.Throws<CardValidationException>(() => 
                new CardDetails(ValidCardNumber, ValidExpiryDate, invalidCvv!));
            
            Assert.Equal("CVV cannot be empty", exception.Message);
        }

        [Theory]
        [InlineData("12")] // Too short
        [InlineData("12345")] // Too long
        [InlineData("abc")] // Letters
        [InlineData("12a")] // Mixed
        public void GivenInvalidCvv_ShouldThrowCardValidationException(String invalidCvv)
        {
            CardValidationException exception = Assert.Throws<CardValidationException>(() => 
                new CardDetails(ValidCardNumber, ValidExpiryDate, invalidCvv));
            
            Assert.Equal("CVV must be 3 or 4 digits", exception.Message);
        }
    }

    public class Clone
    {
        [Fact]
        public void ShouldCreateNewInstanceWithSameValues()
        {
            // Arrange
            CardDetails original = CreateValidCardDetails();

            // Act
            CardDetails clone = original.Clone();

            // Assert
            Assert.NotSame(original, clone);
            Assert.Equal(original.MaskedCardNumber, clone.MaskedCardNumber);
            Assert.Equal(original.ExpiryDate, clone.ExpiryDate);
            Assert.Equal(original.MaskedCvv, clone.MaskedCvv);
        }
    }
} 