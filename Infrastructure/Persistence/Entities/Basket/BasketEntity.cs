namespace Infrastructure.Persistence.Entities.Basket;

public partial class BasketEntity
{
    public Guid Id { get; set; } 
    public Guid CustomerId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public ICollection<BasketItemEntity> Items { get; set; } = [];
}