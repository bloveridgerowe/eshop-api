using System.Text.RegularExpressions;
using Domain.Exceptions.Customers;

namespace Domain.ValueObjects;

public class CardDetails
{
    private static readonly Regex ValidCardNumberPattern = new("^[0-9]{16}$", RegexOptions.Compiled);
    private static readonly Regex ValidExpiryDatePattern = new("^(0[1-9]|1[0-2])/([0-9]{2})$", RegexOptions.Compiled);
    private static readonly Regex ValidCvvPattern = new("^[0-9]{3,4}$", RegexOptions.Compiled);
    
    public String CardNumber { get; }
    public String ExpiryDate { get; }
    public String Cvv { get; }
    
    public String MaskedCardNumber => $"************{CardNumber[^4..]}";
    public String MaskedExpiryDate => $"**/**";
    public String MaskedCvv => new('*', Cvv.Length);

    public CardDetails(String cardNumber, String expiryDate, String cvv)
    {
        ValidateCardNumber(cardNumber);
        ValidateExpiryDate(expiryDate);
        ValidateCvv(cvv);
        
        CardNumber = cardNumber;
        ExpiryDate = expiryDate;
        Cvv = cvv;
    }
    
    private static void ValidateCardNumber(String cardNumber)
    {
        if (String.IsNullOrWhiteSpace(cardNumber))
        {
            throw new CardValidationException("Card number cannot be empty");
        }
        
        if (!ValidCardNumberPattern.IsMatch(cardNumber))
        {
            throw new CardValidationException("Card number must be 16 digits");
        }
    }
    
    private static void ValidateExpiryDate(String expiryDate)
    {
        if (String.IsNullOrWhiteSpace(expiryDate))
        {
            throw new CardValidationException("Expiry date cannot be empty");
        }
        
        if (!ValidExpiryDatePattern.IsMatch(expiryDate))
        {
            throw new CardValidationException("Expiry date must be in MM/YY format");
        }
        
        String[] parts = expiryDate.Split('/');
        Int32 month = int.Parse(parts[0]);
        Int32 year = int.Parse(parts[1]);
        
        DateTime cardExpiry = new DateTime(2000 + year, month, 1).AddMonths(1).AddDays(-1);
        if (cardExpiry < DateTime.UtcNow.Date)
        {
            throw new CardValidationException("Card has expired");
        }
    }
    
    private static void ValidateCvv(String cvv)
    {
        if (String.IsNullOrWhiteSpace(cvv))
        {
            throw new CardValidationException("CVV cannot be empty");
        }
        
        if (!ValidCvvPattern.IsMatch(cvv))
        {
            throw new CardValidationException("CVV must be 3 or 4 digits");
        }
    }

    public CardDetails Clone()
    {
        return new CardDetails(CardNumber, ExpiryDate, Cvv);
    }
}