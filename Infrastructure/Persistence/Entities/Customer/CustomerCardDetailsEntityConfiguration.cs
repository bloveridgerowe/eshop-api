using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Entities.Customer;

public class CustomerCardDetailsEntityConfiguration : IEntityTypeConfiguration<CustomerCardDetailsEntity>
{
    public void Configure(EntityTypeBuilder<CustomerCardDetailsEntity> builder)
    {
        builder.HasKey(cd => cd.Id);

        builder.Property(cd => cd.CardNumber)
            .IsRequired()
            .HasMaxLength(16);

        builder.Property(cd => cd.ExpiryDate)
            .IsRequired()
            .HasMaxLength(5);

        builder.Property(cd => cd.Cvv)
            .IsRequired()
            .HasMaxLength(4);
    }
}