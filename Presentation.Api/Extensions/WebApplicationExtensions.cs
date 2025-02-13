using System.Text;
using Application;
using Application.Interfaces;
using Application.Services;
using Domain.Repositories;
using Domain.Services;
using FastEndpoints;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Entities.Users;
using Infrastructure.Persistence.Repositories;
using Infrastructure.Persistence.Services;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Npgsql;
using Presentation.Api.BackgroundServices;
using Presentation.Api.Middleware;

namespace Presentation.Api.Extensions;

public static class WebApplicationExtensions
{
    public static WebApplication UseGlobalExceptionHandler(this WebApplication webApplication)
    {
        webApplication.UseMiddleware<ExceptionHandlingMiddleware>();
        return webApplication;
    }

    public static WebApplication MapApiControllers(this WebApplication webApplication)
    {
        webApplication.MapControllers();
        return webApplication;
    }

    public static WebApplication EnsureDatabaseExists(this WebApplication webApplication)
    {
        using IServiceScope scope = webApplication.Services.CreateScope();
        using ECommerceDbContext context = scope.ServiceProvider.GetRequiredService<ECommerceDbContext>();
        context.Database.EnsureCreated();
        
        return webApplication;
    }

    public static WebApplicationBuilder SetUrls(this WebApplicationBuilder webApplicationBuilder)
    {
        webApplicationBuilder.WebHost.UseUrls($"http://*:{Environment.GetEnvironmentVariable("PORT") ?? "8081"}");
        return webApplicationBuilder;
    }

    public static WebApplicationBuilder AddConfiguration(this WebApplicationBuilder webApplicationBuilder)
    {
        webApplicationBuilder.Configuration
            .AddJsonFile("appsettings.Development.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{webApplicationBuilder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables();

        return webApplicationBuilder;
    }

    public static WebApplicationBuilder AddFastEndpoints(this WebApplicationBuilder webApplicationBuilder)
    {
        webApplicationBuilder.Services.AddFastEndpoints();

        return webApplicationBuilder;
    }

    public static WebApplicationBuilder AddIdentity(this WebApplicationBuilder webApplicationBuilder)
    {
        webApplicationBuilder.Services.AddAuthorizationBuilder();
        
        webApplicationBuilder.Services
            .AddIdentityCore<ApplicationUser>()
            .AddEntityFrameworkStores<ECommerceDbContext>();

        return webApplicationBuilder;
    }

    public static WebApplicationBuilder ConfigureServices(this WebApplicationBuilder webApplicationBuilder, IConfiguration configuration)
    {
        String NpgsqlConnectionStringFromDatabaseUrl(String databaseUrl)
        {
            Uri railwayDatabaseUri = new Uri(databaseUrl);
            String[] userInfo = railwayDatabaseUri.UserInfo.Split(':');
            
            NpgsqlConnectionStringBuilder builder = new NpgsqlConnectionStringBuilder
            {
                Host = railwayDatabaseUri.Host,
                Port = railwayDatabaseUri.Port,
                Username = userInfo[0],
                Password = userInfo[1],
                Database = railwayDatabaseUri.LocalPath.TrimStart('/'),
                SslMode = SslMode.Require,
                TrustServerCertificate = true
            };
            
            return builder.ToString();
        }
        
        String? connectionString = configuration.GetConnectionString("DefaultConnection");
        String? databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");

        if (connectionString is null && databaseUrl is null)
        {
            throw new Exception("No database configuration provided.");
        }
        
        String? prioritizedConnectionString = String.IsNullOrEmpty(databaseUrl) 
            ? connectionString 
            : NpgsqlConnectionStringFromDatabaseUrl(databaseUrl);
        
        webApplicationBuilder.Services
            .AddDbContext<ECommerceDbContext>(o =>
            {
                o.UseNpgsql(prioritizedConnectionString);
            })
            .AddMediatR(config =>
            {
                config.RegisterServicesFromAssembly(typeof(IApplicationAssemblyMarker).Assembly);
            })
            .AddScoped<IEmailService, MockEmailService>()
            .AddScoped<ITokenService, TokenService>()
            .AddScoped<IAuthenticationService, AuthenticationService>()
            .AddScoped<ICustomerRepository, CustomerRepository>()
            .AddHttpClient()
            .AddScoped<SeedData>()
            .AddScoped<IProductQueryService, ProductQueryService>()
            .AddScoped<IBasketRepository, BasketRepository>()
            .AddScoped<ICategoryRepository, CategoryRepository>()
            .AddScoped<IProductRepository, ProductRepository>()
            .AddScoped<IOrderRepository, OrderRepository>()
            .AddScoped<IUnitOfWork, ECommerceUnitOfWork>()
            .AddControllers();

        return webApplicationBuilder;
    }

    public static WebApplicationBuilder ConfigureCors(this WebApplicationBuilder webApplicationBuilder)
    {
        webApplicationBuilder.Services.AddCors(options =>
        {
            options.AddPolicy("CorsPolicy", b => b
                .SetIsOriginAllowed(origin =>
                    new Uri(origin).Host == "localhost" || 
                    origin == "https://eshop-ui.up.railway.app" ||
                    origin == "https://portfolio-ui.up.railway.app")
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials());
        });

        return webApplicationBuilder;
    }
    
    public static WebApplicationBuilder AddHostedServices(this WebApplicationBuilder webApplicationBuilder)
    {
        webApplicationBuilder.Services
            .AddHostedService<OrderFulfilmentBackgroundService>()
            .AddHostedService<StaleSessionCleanupBackgroundService>();

        return webApplicationBuilder;
    }

    public static async Task<WebApplication> EnsureSeededAsync(this WebApplication webApplication)
    {
        using IServiceScope scope = webApplication.Services.CreateScope();
        IServiceProvider services = scope.ServiceProvider;
        
        SeedData seedData = services.GetRequiredService<SeedData>();
        ECommerceDbContext eCommerceDbContext = services.GetRequiredService<ECommerceDbContext>();

        if (!eCommerceDbContext.Customers.Any())
        {
            await seedData.SeedAsync();
        }

        return webApplication;
    }

    public static void ForwardAllHeaders(this WebApplication webApplication)
    {
        webApplication.UseForwardedHeaders(new ForwardedHeadersOptions
        {
            ForwardedHeaders = ForwardedHeaders.All
        });
    }
    
    public static void AddHttpContextAccessor(this WebApplicationBuilder webApplicationBuilder)
    {
        webApplicationBuilder.Services.AddHttpContextAccessor();;
    }

    public static WebApplicationBuilder ConfigureJwt(this WebApplicationBuilder builder, IConfiguration configuration)
    {
        IConfigurationSection jwtSettings = configuration.GetSection("JwtSettings");
        String secretKey = Environment.GetEnvironmentVariable("ACCESS_TOKEN_SECRET")!;

        builder.Services
            .AddAuthentication(opt =>
            {
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings["validIssuer"],
                    ValidAudience = jwtSettings["validAudience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
                };

                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = c =>
                    {
                        String? accessToken = c.Request.Cookies[AuthenticationService.AccessTokenCookieName];

                        if (!String.IsNullOrEmpty(accessToken))
                        {
                            c.Token = accessToken;
                        }

                        return Task.CompletedTask;
                    }
                };
            });
        
        return builder;
    }
}