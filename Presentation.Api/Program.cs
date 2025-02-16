using FastEndpoints;
using Presentation.Api.Extensions;

namespace Presentation.Api;

public class Program
{
    public static async Task Main(String[] args)
    {
        WebApplicationBuilder webApplicationBuilder = WebApplication.CreateBuilder(args);
        webApplicationBuilder.ConfigureCors();
        webApplicationBuilder.AddConfiguration();
        webApplicationBuilder.ConfigureServices(webApplicationBuilder.Configuration);
        webApplicationBuilder.AddHttpContextAccessor();
        webApplicationBuilder.AddIdentity();
        webApplicationBuilder.ConfigureJwt(webApplicationBuilder.Configuration);
        webApplicationBuilder.AddFastEndpoints();
        webApplicationBuilder.SetUrls();
        webApplicationBuilder.AddHostedServices();
        
        WebApplication webApplication = webApplicationBuilder.Build();
        webApplication.EnsureDatabaseExists();
        await webApplication.EnsureSeededAsync();
        webApplication.UseGlobalExceptionHandler();
        webApplication.UseFastEndpoints();
        webApplication.ForwardAllHeaders();
        webApplication.UseCors("CorsPolicy");
        webApplication.UseAuthentication();
        webApplication.UseAuthorization();
        webApplication.Run();
    }
}