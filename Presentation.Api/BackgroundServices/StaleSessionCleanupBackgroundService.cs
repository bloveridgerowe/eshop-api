using Infrastructure.Persistence;
using Infrastructure.Persistence.Entities.Users;
using Microsoft.EntityFrameworkCore;

namespace Presentation.Api.BackgroundServices;

public class StaleSessionCleanupBackgroundService : BackgroundService
{
    // Background services are singletons so we must inject a service scope factory to retrieve our scoped items
    
    private readonly ILogger<StaleSessionCleanupBackgroundService> _logger;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    
    public StaleSessionCleanupBackgroundService(ILogger<StaleSessionCleanupBackgroundService> logger, IServiceScopeFactory serviceScopeFactory)
    {
        _logger = logger;
        _serviceScopeFactory = serviceScopeFactory;
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Sometimes users will just clear their cookies and not logout property via the web UI
        // In these cases, the backend will never be notified of their logout, so the session entry won't be removed
        // This will leave behind stale sessions in the ApplicationUserSession table
        // These stale sessions will not cause operational issues, but will end up consuming unnecessary space
        // So we will clear this out periodically to keep the table clean
        
        _logger.LogInformation("Stale session cleanup background Service is starting");
        
        PeriodicTimer timer = new PeriodicTimer(TimeSpan.FromHours(1));

        while (!stoppingToken.IsCancellationRequested && await timer.WaitForNextTickAsync(stoppingToken))
        {
            try
            {
                _logger.LogInformation("Clearing stale sessions");
            
                await CleanupStaleSessions();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Stale session cleanup background service failed");
            }
        }
    }

    private async Task CleanupStaleSessions()
    {
        await using AsyncServiceScope asyncScope = _serviceScopeFactory.CreateAsyncScope();
        ECommerceDbContext eCommerceDbContext = asyncScope.ServiceProvider.GetRequiredService<ECommerceDbContext>();
        
        List<ApplicationUserSession> staleSessions = await eCommerceDbContext.ApplicationUserSessions
            .Where(s => s.RefreshTokenExpiryTime < DateTime.UtcNow)
            .ToListAsync();
        
        eCommerceDbContext.ApplicationUserSessions.RemoveRange(staleSessions);
        await eCommerceDbContext.SaveChangesAsync();
        
        _logger.LogInformation("Cleared {sessionsCleared} stale sessions", staleSessions.Count);
    }
}