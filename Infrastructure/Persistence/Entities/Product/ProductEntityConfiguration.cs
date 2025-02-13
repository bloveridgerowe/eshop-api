using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Infrastructure.Persistence.Entities.Category;

namespace Infrastructure.Persistence.Entities.Product;

public class ProductEntityConfiguration : IEntityTypeConfiguration<ProductEntity>
{
    public void Configure(EntityTypeBuilder<ProductEntity> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(p => p.Description)
            .IsRequired();
        
        builder.Property(p => p.Price)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(p => p.Stock)
            .IsRequired();
        
        builder.Property(p => p.ImageUrl)
            .IsRequired();

        builder.HasMany(p => p.Categories)
            .WithMany(c => c.Products)
            .UsingEntity<Dictionary<String, object>>(
                "ProductCategory",
                j => j.HasOne<CategoryEntity>().WithMany().HasForeignKey("CategoryId"),
                j => j.HasOne<ProductEntity>().WithMany().HasForeignKey("ProductId")
            );
    }
}