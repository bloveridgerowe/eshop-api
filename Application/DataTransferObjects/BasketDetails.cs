namespace Application.DataTransferObjects;

public class BasketDetails
{
    public Guid Id { get; init; }
    public Guid CustomerId { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
    public List<BasketItemDetails> Items { get; init; } = [];
    public Decimal TotalPrice { get; init; }
}