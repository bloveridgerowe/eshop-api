namespace Infrastructure.Persistence.Entities.Users;

public class ApplicationUserSession
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public String RefreshTokenHash { get; set; } = null!;
    public DateTime RefreshTokenExpiryTime { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public String? Device { get; set; } = null!;
    public String? IpAddress { get; set; } = null!;
    public ApplicationUser User { get; set; } = null!;
}