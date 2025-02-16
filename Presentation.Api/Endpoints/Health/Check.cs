using FastEndpoints;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Presentation.Api.Endpoints.Health;

[AllowAnonymous]
[HttpGet("/health/check")]
public class Check : EndpointWithoutRequest
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ECommerceDbContext _dbContext;

    public Check(IHttpClientFactory httpClientFactory, ECommerceDbContext dbContext)
    {
        _httpClientFactory = httpClientFactory;
        _dbContext = dbContext;
    }

    public override async Task HandleAsync(CancellationToken cancellationToken)
    {
        if (!await CheckDatabaseAvailable(cancellationToken) || !await CheckWebsiteAvailable(cancellationToken))
        {
            await SendNotFoundAsync(cancellationToken);
            return;
        }

        await SendOkAsync(cancellationToken);
    }

    public async Task<Boolean> CheckDatabaseAvailable(CancellationToken cancellationToken)
    {
        try
        {
            _ = await _dbContext.Products.AnyAsync(cancellationToken);
            return true;
        }
        catch
        {
            return false;
        }
    }

    async Task<Boolean> CheckWebsiteAvailable(CancellationToken cancellationToken)
    {
        try
        {
            HttpClient httpClient = _httpClientFactory.CreateClient();
            HttpResponseMessage response = await httpClient.GetAsync("https://eshop-ui.up.railway.app/products?featured=true", cancellationToken);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }
}