using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Entities.Basket;

public class BasketItemEntityConfiguration : IEntityTypeConfiguration<BasketItemEntity>
{
    public void Configure(EntityTypeBuilder<BasketItemEntity> builder)
    {
        builder.HasKey(i => i.Id);

        builder.Property(i => i.Id)
            .ValueGeneratedOnAdd();

        builder.Property(i => i.BasketId)
            .IsRequired();

        builder.Property(i => i.ProductId)
            .IsRequired();

        builder.Property(i => i.Quantity)
            .IsRequired();

        builder.Property(i => i.Price)
            .HasColumnType("decimal(18,2)") 
            .IsRequired();
        
        builder.HasOne(i => i.Product)
            .WithMany()
            .HasForeignKey(i => i.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.ToTable("BasketItems");
    }
}