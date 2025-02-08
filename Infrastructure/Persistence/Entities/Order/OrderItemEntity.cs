using Infrastructure.Persistence.Entities.Product;

namespace Infrastructure.Persistence.Entities.Order;

public class OrderItemEntity
{
    public Guid Id { get; set; }
    public Guid OrderId { get; set; }
    public Guid ProductId { get; set; }
    public Decimal Price { get; set; }
    public Int32 Quantity { get; set; }
    public ProductEntity Product { get; set; } = null!;
}