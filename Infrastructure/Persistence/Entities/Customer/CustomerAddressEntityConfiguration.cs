using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Entities.Customer;

public class CustomerAddressEntityConfiguration : IEntityTypeConfiguration<CustomerAddressEntity>
{
    public void Configure(EntityTypeBuilder<CustomerAddressEntity> builder)
    {
        builder.HasKey(cd => cd.Id);

        builder.Property(cd => cd.FirstLine)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(cd => cd.SecondLine)
            .IsRequired()
            .HasMaxLength(100);
        
        builder.Property(cd => cd.City)
            .IsRequired()
            .HasMaxLength(100);
        
        builder.Property(cd => cd.County)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(cd => cd.PostCode)
            .IsRequired()
            .HasMaxLength(8);
    }
}