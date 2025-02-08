using System.Security.Claims;

namespace Application.Interfaces;

public interface ITokenService
{
    String CreateAccessToken(List<Claim> claims);
    String CreateRefreshToken();
}