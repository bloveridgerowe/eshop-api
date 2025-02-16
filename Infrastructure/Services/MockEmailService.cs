using Domain.Services;
using Domain.ValueObjects;

namespace Infrastructure.Services;

public class MockEmailService : IEmailService
{
    public async Task SendEmailAsync(Email email)
    {
    }
}