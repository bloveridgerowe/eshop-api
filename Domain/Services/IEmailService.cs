using Domain.ValueObjects;

namespace Domain.Services;

public interface IEmailService
{
    Task SendEmailAsync(Email email);
}