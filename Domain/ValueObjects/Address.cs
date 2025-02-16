using System.Text.RegularExpressions;
using Domain.Exceptions.Customers;

namespace Domain.ValueObjects;

public class Address
{
    private static readonly Regex ValidPostCodePattern = new("^[A-Za-z0-9 ]{6,8}$", RegexOptions.Compiled);

    public String FirstLine { get; }
    public String SecondLine { get; }
    public String City { get; }
    public String County { get; }
    public String PostCode { get; }

    public Address(String firstLine, String secondLine, String city, String county, String postCode)
    {
        ValidateFirstLine(firstLine);
        ValidateSecondLine(secondLine);
        ValidateCity(city);
        ValidateCounty(county);
        ValidatePostCode(postCode);

        FirstLine = firstLine.Trim();
        SecondLine = secondLine.Trim();
        City = city.Trim();
        County = county.Trim();
        PostCode = postCode.Trim().ToUpper();
    }

    private static void ValidateFirstLine(String firstLine)
    {
        if (String.IsNullOrWhiteSpace(firstLine))
        {
            throw new AddressValidationException("Address first line cannot be empty.");
        }

        if (firstLine.Length > 100)
        {
            throw new AddressValidationException("Address first line cannot exceed 100 characters.");
        }
    }

    private static void ValidateSecondLine(String secondLine)
    {
        if (secondLine != null && secondLine.Length > 100)
        {
            throw new AddressValidationException("Address second line cannot exceed 100 characters.");
        }
    }

    private static void ValidateCity(String city)
    {
        if (String.IsNullOrWhiteSpace(city))
        {
            throw new AddressValidationException("City cannot be empty.");
        }

        if (city.Length > 50)
        {
            throw new AddressValidationException("City name cannot exceed 50 characters.");
        }
    }

    private static void ValidateCounty(String county)
    {
        if (String.IsNullOrWhiteSpace(county))
        {
            throw new AddressValidationException("County cannot be empty.");
        }

        if (county.Length > 50)
        {
            throw new AddressValidationException("County name cannot exceed 50 characters.");
        }
    }

    private static void ValidatePostCode(String postCode)
    {
        if (String.IsNullOrWhiteSpace(postCode))
        {
            throw new AddressValidationException("Postcode cannot be empty.");
        }

        if (!ValidPostCodePattern.IsMatch(postCode))
        {
            throw new AddressValidationException("Invalid postcode format.");
        }
    }

    public Address Clone()
    {
        return new Address(FirstLine, SecondLine, City, County, PostCode);
    }
}