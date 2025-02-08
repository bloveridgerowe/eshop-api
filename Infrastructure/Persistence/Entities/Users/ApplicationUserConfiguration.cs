using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Entities.Users;

public class ApplicationUserSessionConfiguration : IEntityTypeConfiguration<ApplicationUserSession>
{
    public void Configure(EntityTypeBuilder<ApplicationUserSession> builder)
    {
        builder
            .HasIndex(u => u.RefreshTokenHash)
            .IsUnique();

        builder
            .HasOne(us => us.User)
            .WithMany()
            .HasForeignKey(us => us.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}