using Infrastructure.Persistence.Entities.Basket;
using Infrastructure.Persistence.Entities.Category;
using Infrastructure.Persistence.Entities.Customer;
using Infrastructure.Persistence.Entities.Order;
using Infrastructure.Persistence.Entities.Product;
using Infrastructure.Persistence.Entities.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public class ECommerceDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
{
    public DbSet<CustomerAddressEntity> CustomerAddress { get; set; }
    public DbSet<CustomerCardDetailsEntity> CustomerCardDetails { get; set; }
    public DbSet<CategoryEntity> Categories { get; set; }
    public DbSet<CustomerEntity> Customers { get; set; }
    public DbSet<ProductEntity> Products { get; set; }
    public DbSet<OrderEntity> Orders { get; set; }
    public DbSet<OrderItemEntity> OrderItems { get; set; }
    public DbSet<BasketEntity> Baskets { get; set; }
    public DbSet<BasketItemEntity> BasketItems { get; set; }
    public DbSet<ApplicationUserSession> ApplicationUserSessions { get; set; }
    
    public ECommerceDbContext(DbContextOptions<ECommerceDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
            
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ECommerceDbContext).Assembly);
    }
}