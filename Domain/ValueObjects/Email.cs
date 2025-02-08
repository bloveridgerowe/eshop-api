using System.Text.RegularExpressions;
using Domain.Exceptions.Customers;

namespace Domain.ValueObjects;

public class Email
{
    private static readonly Regex ValidEmailPattern = 
        new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

    public String Address { get; }
    public String Subject { get; }
    public String Body { get; }

    public Email(String address, String subject, String body)
    {
        ValidateAddress(address);
        ValidateSubject(subject);
        ValidateBody(body);

        Address = address.Trim();
        Subject = subject.Trim();
        Body = body.Trim();
    }

    private static void ValidateAddress(String address)
    {
        if (String.IsNullOrWhiteSpace(address))
        {
            throw new EmailValidationException("Email address cannot be empty.");
        }

        if (!ValidEmailPattern.IsMatch(address))
        {
            throw new EmailValidationException("Invalid email address format.");
        }
    }

    private static void ValidateSubject(String subject)
    {
        if (String.IsNullOrWhiteSpace(subject))
        {
            throw new EmailValidationException("Email subject cannot be empty.");
        }

        if (subject.Length > 200)
        {
            throw new EmailValidationException("Email subject cannot exceed 200 characters.");
        }
    }

    private static void ValidateBody(String body)
    {
        if (String.IsNullOrWhiteSpace(body))
        {
            throw new EmailValidationException("Email body cannot be empty.");
        }

        if (body.Length > 5000)
        {
            throw new EmailValidationException("Email body cannot exceed 5000 characters.");
        }
    }

    public Email Clone()
    {
        return new Email(Address, Subject, Body);
    }
}