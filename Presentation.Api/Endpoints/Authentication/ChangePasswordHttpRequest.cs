namespace Presentation.Api.Endpoints.Authentication;

public class ChangePasswordHttpRequest
{
    public required String OldPassword { get; init; }
    public required String NewPassword { get; init; }
}