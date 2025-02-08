using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Entities.Basket;

public class BasketEntityConfiguration : IEntityTypeConfiguration<BasketEntity>
{
    public void Configure(EntityTypeBuilder<BasketEntity> builder)
    {
        builder.HasKey(b => b.Id);

        builder.Property(b => b.Id)
            .ValueGeneratedNever();

        builder.Property(b => b.CustomerId)
            .IsRequired();

        builder.Property(b => b.CreatedAt)
            .IsRequired();

        builder.Property(b => b.UpdatedAt)
            .IsRequired();

        builder.HasMany(b => b.Items)
            .WithOne()
            .HasForeignKey(i => i.BasketId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}