namespace Application.DataTransferObjects;

public class BasketItemDetails
{
    public Guid ProductId { get; init; }
    public String Name { get; init; }
    public Int32 Quantity { get; init; }
    public Decimal Price { get; init; }
    public Decimal TotalPrice => Price * Quantity;
}