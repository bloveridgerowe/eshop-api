namespace Application.DataTransferObjects;

public class ProductSummary
{
    public Guid Id { get; init; }
    public String Name { get; init; }
    public Decimal Price { get; init; }
    public Int32 Stock { get; init; }
    public Boolean Featured { get; init; }
    public String ImageUrl { get; init; }
    public List<String> Categories { get; init; }
}