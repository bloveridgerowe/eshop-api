using System.Text.RegularExpressions;
using Domain.Exceptions.Customers;
using Domain.ValueObjects;

namespace Domain.Entities;

public class Customer : Entity
{
    private static readonly Regex ValidEmailPattern = new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled);
    private const Int32 MaxNameLength = 100;
    private const Int32 MaxAddressLength = 500;
    
    public String Email { get; }
    public String? Name { get; private set; }
    public Address? Address { get; private set; }
    public CardDetails? CardDetails { get; private set; }

    public Customer(Guid id, String email, String? name = null, Address? address = null, CardDetails? cardDetails = null)
        : base(id)
    {
        ValidateEmail(email);
        
        if (name is not null)
        {
            ValidateName(name);
        }
        
        Email = email;
        Name = name;
        Address = address;
        CardDetails = cardDetails?.Clone();
    }
    
    public Customer(Guid id, String email)
        : this(id, email, default, default, default)
    {
    }

    public void UpdateName(String? newName)
    {
        if (newName is not null)
        {
            ValidateName(newName);
        }
        
        Name = newName;
    }

    public void UpdateAddress(Address newAddress)
    {
        Address = newAddress;
    }

    public void UpdateCardDetails(CardDetails? newCardDetails)
    {
        CardDetails = newCardDetails?.Clone();
    }
    
    private static void ValidateEmail(String email)
    {
        if (String.IsNullOrWhiteSpace(email))
        {
            throw new CustomerValidationException("Email cannot be empty");
        }
        
        if (!ValidEmailPattern.IsMatch(email))
        {
            throw new CustomerValidationException("Invalid email format");
        }
        
        if (email.Length > 254) // RFC 5321
        {
            throw new CustomerValidationException("Email is too long");
        }
    }
    
    private static void ValidateName(String name)
    {
        if (String.IsNullOrWhiteSpace(name))
        {
            throw new CustomerValidationException("Name cannot be empty");
        }
        
        if (name.Length > MaxNameLength)
        {
            throw new CustomerValidationException($"Name cannot be longer than {MaxNameLength} characters");
        }
        
        if (!name.All(c => char.IsLetter(c) || char.IsWhiteSpace(c) || c == '-' || c == '\''))
        {
            throw new CustomerValidationException("Name can only contain letters, spaces, hyphens and apostrophes");
        }
    }
    
    private static void ValidateAddress(String address)
    {
        if (String.IsNullOrWhiteSpace(address))
        {
            throw new CustomerValidationException("Address cannot be empty");
        }
        
        if (address.Length > MaxAddressLength)
        {
            throw new CustomerValidationException($"Address cannot be longer than {MaxAddressLength} characters");
        }
        
        if (address.Any(c => char.IsControl(c)))
        {
            throw new CustomerValidationException("Address cannot contain control characters");
        }
    }
}