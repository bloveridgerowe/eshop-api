namespace Domain.Aggregates.Orders;

public class OrderProcessingStatus
{
    public Int32 Id { get; }
    public String Name { get; }

    // Private constructors means no external callers can create their own order processing statuses
    
    private OrderProcessingStatus()
    {
    }

    private OrderProcessingStatus(Int32 id, String name)
    {
        Id = id;
        Name = name;
    }
    
    public static readonly OrderProcessingStatus Pending = new(1, "Pending");
    public static readonly OrderProcessingStatus Shipped = new(2, "Shipped");
    public static readonly OrderProcessingStatus Delivered = new(3, "Delivered");
    public static readonly OrderProcessingStatus Canceled = new(4, "Canceled");

    public static readonly List<OrderProcessingStatus> Statuses = [ Pending, Shipped, Delivered, Canceled ];
}