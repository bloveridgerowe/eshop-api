using Infrastructure.Persistence.Entities.Product;

namespace Infrastructure.Persistence.Entities.Basket;

public class BasketItemEntity
{
    public Guid Id { get; set; }
    public Guid BasketId { get; set; }
    public Guid ProductId { get; set; }
    public Int32 Quantity { get; set; }
    public Decimal Price { get; set; }
    public ProductEntity Product { get; set; } = null!;
}