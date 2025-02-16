namespace Domain.Exceptions.Products;

public class InsufficientStockException : DomainException
{
    public String ProductName { get; }
    public Int32 Requested { get; }
    public Int32 Available { get; }

    public InsufficientStockException(String productName, Int32 requested, Int32 available)
        : base(
            $"Insufficient stock for product '{productName}'. Requested: {requested}, Available: {available}")
    {
        ProductName = productName;
        Requested = requested;
        Available = available;
    }
}
