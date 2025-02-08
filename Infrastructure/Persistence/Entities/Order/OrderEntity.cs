namespace Infrastructure.Persistence.Entities.Order;

public partial class OrderEntity
{
    public Guid Id { get; set; }
    public Guid CustomerId { get; set; }
    public String Status { get; set; } = null!;
    public List<OrderItemEntity> Items { get; set; } = [];
    public DateTime CreatedAt { get; set; }
}