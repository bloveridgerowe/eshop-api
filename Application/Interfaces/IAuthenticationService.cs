using Application.DataTransferObjects;

namespace Application.Interfaces;

public interface IAuthenticationService
{
    Task ChangeUserPassword(String oldPassword, String newPassword);
    Task<Guid> RegisterUser(String email, String password);
    Task<Boolean> ValidateUser(String email, String password);
    Task<Boolean> ValidateRefreshToken(String refreshToken);
    Task<TokenSet> CreateTokensForUser(String email, String password);
    Task<TokenSet> CreateTokensForUser(String refreshToken);
    Task RevokeUserSession(String refreshToken);
    void SetTokenCookies(TokenSet applicationTokens);
    void ClearTokenCookies();
    void LogTokenCookies();
    String? GetAccessToken();
    String? GetRefreshToken();
}