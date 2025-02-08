using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Entities.Customer;

public class CustomerEntityConfiguration : IEntityTypeConfiguration<CustomerEntity>
{
    public void Configure(EntityTypeBuilder<CustomerEntity> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Name)
            .IsRequired(false)
            .HasMaxLength(100);

        builder.Property(c => c.Email)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasOne(c => c.Address)
            .WithOne()
            .HasForeignKey<CustomerAddressEntity>(cd => cd.CustomerId)
            .OnDelete(DeleteBehavior.Cascade);;
        
        builder.HasOne(c => c.CardDetails)
            .WithOne()
            .HasForeignKey<CustomerCardDetailsEntity>(cd => cd.CustomerId)
            .OnDelete(DeleteBehavior.Cascade);;
    }
}