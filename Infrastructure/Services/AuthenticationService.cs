using System.Security.Authentication;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Application.DataTransferObjects;
using Application.Interfaces;
using Infrastructure.Extensions;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Entities.Users;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;

namespace Infrastructure.Services;

public sealed class AuthenticationService : IAuthenticationService
{
    public static readonly String AccessTokenCookieName = "accessToken"; 
    public static readonly String RefreshTokenCookieName = "refreshToken"; 
    
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ITokenService _tokenService;
    private readonly ILogger<AuthenticationService> _logger;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ECommerceDbContext _dbContext;

    public AuthenticationService(IHttpContextAccessor httpContextAccessor, ITokenService tokenService, ILogger<AuthenticationService> logger, UserManager<ApplicationUser> userManager, ECommerceDbContext dbContext)
    {
        _httpContextAccessor = httpContextAccessor;
        _tokenService = tokenService;
        _logger = logger;
        _userManager = userManager;
        _dbContext = dbContext;
    }

    public async Task ChangeUserPassword(String oldPassword, String newPassword)
    {
        ClaimsPrincipal? userPrincipal = _httpContextAccessor.HttpContext?.User;
    
        if (userPrincipal is null)
        {
            throw new AuthenticationException("User is not authenticated.");
        }

        ApplicationUser? user = await _userManager.GetUserAsync(userPrincipal);

        if (user is null)
        {
            throw new AuthenticationException("User not found.");
        }

        IdentityResult result = await _userManager.ChangePasswordAsync(user, oldPassword, newPassword);

        if (!result.Succeeded)
        {
            throw new AuthenticationException($"Failed to change password: {String.Join(", ", result.Errors.Select(e => e.Description))}");
        }

        _dbContext.ApplicationUserSessions.RemoveRange(_dbContext.ApplicationUserSessions.Where(us => us.UserId == user.Id));
        await _dbContext.SaveChangesAsync();
    }

    public async Task<Guid> RegisterUser(String email, String password)
    {
        if (await _userManager.FindByEmailAsync(email) is not null)
        {
            throw new AuthenticationException("User with this email already exists.");
        }

        ApplicationUser user = new()
        {
            Id = Guid.NewGuid(),
            Email = email,
            UserName = email,
        };

        IdentityResult result = await _userManager.CreateAsync(user, password);
    
        if (!result.Succeeded)
        {
            throw new AuthenticationException($"Failed to register user: {String.Join(", ", result.Errors.Select(e => e.Description))}");
        }

        return user.Id;
    }

    public async Task<Boolean> ValidateUser(String email, String password)
    {
        ApplicationUser? applicationUser = await _userManager.FindByEmailAsync(email);

        if (applicationUser is not null && await _userManager.CheckPasswordAsync(applicationUser, password))
        {
            return true;
        }
        
        return false;
    }

    public async Task<Boolean> ValidateRefreshToken(String refreshToken)
    {
        String hashedRefreshToken = HashRefreshToken(refreshToken);
        
        return await _dbContext.ApplicationUserSessions.AnyAsync(us =>
            us.RefreshTokenHash == hashedRefreshToken &&
            us.RefreshTokenExpiryTime > DateTime.UtcNow);
    }
    
    private List<Claim> GetClaimsForUser(ApplicationUser applicationUser)
    {
        List<Claim> claims =
        [
            new Claim(ClaimTypes.Name, applicationUser.UserName!),
            new Claim(ClaimTypes.NameIdentifier, applicationUser.Id.ToString())
        ];

        return claims;
    }
    
    private String HashRefreshToken(String refreshToken)
    {
        String secret = Environment.GetEnvironmentVariable("REFRESH_TOKEN_SECRET")!;
        Byte[] key = Encoding.UTF8.GetBytes(secret);
        
        using HMACSHA256 hmac = new HMACSHA256(key);
        return Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(refreshToken)));
    }
    
    public async Task<TokenSet> CreateTokensForUser(String email, String password)
    {
        ApplicationUser? applicationUser = await _userManager.FindByEmailAsync(email);

        if (applicationUser is null || !await _userManager.CheckPasswordAsync(applicationUser, password))
        {
            throw new AuthenticationException("Invalid credentials");
        }

        List<Claim> claims = GetClaimsForUser(applicationUser);

        String accessToken = _tokenService.CreateAccessToken(claims);
        
        String refreshToken = _tokenService.CreateRefreshToken();
        String hashedRefreshToken = HashRefreshToken(refreshToken);

        ApplicationUserSession newUserSession = new ApplicationUserSession
        {
            UserId = applicationUser.Id,
            CreatedAt = DateTime.UtcNow,
            RefreshTokenHash = hashedRefreshToken,
            RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7),
            Device = GetUserAgent(),
            IpAddress = GetIpAddress()
        };

        _dbContext.ApplicationUserSessions.Add(newUserSession);
        await _dbContext.SaveChangesAsync();

        return new TokenSet
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
        };
    }

    public async Task<TokenSet> CreateTokensForUser(String refreshToken)
    {
        String hashedRefreshToken = HashRefreshToken(refreshToken);
        
        try
        {
            await using IDbContextTransaction transaction = await _dbContext.Database.BeginTransactionAsync();

            ApplicationUserSession? oldUserSession = await _dbContext.ApplicationUserSessions.SingleOrDefaultAsync(us => us.RefreshTokenHash == hashedRefreshToken);
            
            if (oldUserSession == null)
            {
                throw new AuthenticationException("Failed to find refresh token");
            }

            if (oldUserSession.RefreshTokenExpiryTime <= DateTime.UtcNow)
            {
                throw new AuthenticationException("Expired refresh token");
            }

            ApplicationUser? applicationUser = await _userManager.Users.SingleOrDefaultAsync(u => u.Id == oldUserSession.UserId);

            if (applicationUser == null)
            {
                throw new AuthenticationException("User not found");
            }

            _dbContext.ApplicationUserSessions.Remove(oldUserSession);

            List<Claim> claims = GetClaimsForUser(applicationUser);
            
            String newAccessToken = _tokenService.CreateAccessToken(claims);
            
            String newRefreshToken = _tokenService.CreateRefreshToken();
            String hashedNewRefreshToken = HashRefreshToken(newRefreshToken);

            ApplicationUserSession newUserSession = new ApplicationUserSession
            {
                UserId = applicationUser.Id,
                CreatedAt = DateTime.UtcNow,
                RefreshTokenHash = hashedNewRefreshToken,
                RefreshTokenExpiryTime = oldUserSession.RefreshTokenExpiryTime,
                Device = GetUserAgent(),
                IpAddress = GetIpAddress()
            };

            _dbContext.ApplicationUserSessions.Add(newUserSession);
            await _dbContext.SaveChangesAsync();

            await transaction.CommitAsync();
            
            LogRefreshAttempt(hashedRefreshToken, true, "Deleted old session, created new session, issued new refresh token");

            return new TokenSet
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken
            };
        }
        catch (Exception e)
        {
            LogRefreshAttempt(hashedRefreshToken, false, e.Message);

            throw;
        }
    }

    private void LogRefreshAttempt(String hashedRefreshToken, Boolean successful, String message)
    {
        _logger.LogInformation("Refresh attempt from device with UserAgent: '{userAgent}', IpAddress: {ipAddress}, HashedRefreshToken: '{hashedRefreshToken}', Successful: '{successful}', Message: '{message}'",
            GetUserAgent(), GetIpAddress(), hashedRefreshToken, successful, message);
    }

    public void ClearTokenCookies()
    {
        _httpContextAccessor.HttpContext?.Response.Cookies.Delete(AccessTokenCookieName);
        _httpContextAccessor.HttpContext?.Response.Cookies.Delete(RefreshTokenCookieName);
    }
    
    private void SetCookie(String name, String value, DateTimeOffset expires)
    {
        _httpContextAccessor.HttpContext?.Response.Cookies.Append(name, value, new CookieOptions
        {
            Expires = expires,
            HttpOnly = true,
            IsEssential = true,
            Secure = true,
            SameSite = SameSiteMode.Lax
        });
    }
    
    public async Task RevokeUserSession(String refreshToken)
    {
        String hashedRefreshToken = HashRefreshToken(refreshToken);
        
        List<ApplicationUserSession> sessions = await _dbContext.ApplicationUserSessions.Where(us => us.RefreshTokenHash == hashedRefreshToken).ToListAsync();
    
        if (sessions.Any())
        {
            _dbContext.ApplicationUserSessions.RemoveRange(sessions);
            await _dbContext.SaveChangesAsync();
        }
    }

    public void LogTokenCookies()
    {
        _logger.LogInformation("UserId: {userId}, UserAgent: {userAgent}, IpAddress: {ipAddress}, AccessTokenCookie: {accessToken}, RefreshTokenCookie: {refreshToken}", 
            GetUserId(), GetUserAgent(), GetIpAddress(), GetAccessToken(), GetRefreshToken());
    }

    public void SetTokenCookies(TokenSet tokenSet)
    {
        SetCookie(AccessTokenCookieName, tokenSet.AccessToken, DateTimeOffset.UtcNow.AddMinutes(5));
        SetCookie(RefreshTokenCookieName, tokenSet.RefreshToken, DateTimeOffset.UtcNow.AddDays(7));
    }

    private String? GetUserAgent()
    {
        return _httpContextAccessor.HttpContext?.Request.Headers.UserAgent.ToString();
    }

    private String? GetIpAddress()
    {
        return _httpContextAccessor.HttpContext?.Request.Headers["X-Forwarded-For"].ToString();
    }

    private Guid? GetUserId()
    {
        return _httpContextAccessor.HttpContext?.User.GetId();
    }
    
    public String? GetAccessToken()
    {
        return _httpContextAccessor.HttpContext?.Request.Cookies[AccessTokenCookieName];
    }

    public String? GetRefreshToken()
    {
        return _httpContextAccessor.HttpContext?.Request.Cookies[RefreshTokenCookieName];
    }
}