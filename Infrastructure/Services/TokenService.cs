using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure.Services;

public class TokenService : ITokenService
{
    private readonly IConfiguration _configuration;

    public TokenService(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    public String CreateAccessToken(List<Claim> claims)
    {
        SigningCredentials signingCredentials = GetSigningCredentials();
        JwtSecurityToken tokenOptions = GenerateTokenOptions(signingCredentials, claims);

        String? accessToken = new JwtSecurityTokenHandler().WriteToken(tokenOptions);

        return accessToken;
    }

    public String CreateRefreshToken()
    {
        Byte[] randomNumber = new Byte[32];
        using RandomNumberGenerator randomNumberGenerator = RandomNumberGenerator.Create();
        randomNumberGenerator.GetBytes(randomNumber);

        return Convert.ToBase64String(randomNumber);
    }
    
    private SigningCredentials GetSigningCredentials()
    {
        String secret = Environment.GetEnvironmentVariable("ACCESS_TOKEN_SECRET")!;
        Byte[] key = Encoding.UTF8.GetBytes(secret);
        SymmetricSecurityKey securityKey = new SymmetricSecurityKey(key);

        return new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
    }
    
    private JwtSecurityToken GenerateTokenOptions(SigningCredentials signingCredentials, List<Claim> claims)
    {
        IConfigurationSection jwtSettings = _configuration.GetSection("JwtSettings");

        JwtSecurityToken tokenOptions = new JwtSecurityToken
        (
            issuer: jwtSettings["validIssuer"],
            audience: jwtSettings["validAudience"],
            claims: claims,
            expires: DateTime.Now.AddMinutes(Convert.ToDouble(jwtSettings["expires"])),
            signingCredentials: signingCredentials
        );

        return tokenOptions;
    }
}