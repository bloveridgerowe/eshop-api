using Domain.Exceptions.Orders;

namespace Domain.Aggregates.Orders;

public class OrderItem
{
    private const Int32 MaxQuantity = 99;
    private const Decimal MinPrice = 0.01m;
    
    public Guid ProductId { get; }
    public String Name { get; }
    public Decimal Price { get; private set; }
    public Int32 Quantity { get; private set; }
    public Decimal TotalPrice => Price * Quantity;

    public OrderItem(Guid productId, Decimal price, Int32 quantity, String name)
    {
        ValidateProductId(productId);
        ValidatePrice(price);
        ValidateQuantity(quantity);
        ValidateName(name);
        
        ProductId = productId;
        Price = price;
        Quantity = quantity;
        Name = name;
    }

    private void ValidateName(String name)
    {
        if (String.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Product name cannot be null, empty, or whitespace.", nameof(name));
        }
    }

    public void UpdateQuantity(Int32 quantity)
    {
        ValidateQuantity(quantity);
        Quantity = quantity;
    }

    public void IncreaseQuantity(Int32 quantity)
    {
        ValidatePositiveQuantity(quantity);
        
        Int32 newQuantity = Quantity + quantity;
        ValidateQuantity(newQuantity);
        
        Quantity = newQuantity;
    }

    public void DecreaseQuantity(Int32 quantity)
    {
        ValidatePositiveQuantity(quantity);
        
        Int32 newQuantity = Quantity - quantity;
        ValidateQuantity(newQuantity);
        
        Quantity = newQuantity;
    }

    public void UpdatePrice(Decimal price)
    {
        ValidatePrice(price);
        Price = price;
    }
    
    private static void ValidateProductId(Guid productId)
    {
        if (productId == Guid.Empty)
        {
            throw new OrderValidationException("Product ID cannot be empty");
        }
    }
    
    private static void ValidatePrice(Decimal price)
    {
        if (price < MinPrice)
        {
            throw new OrderValidationException($"Price must be at least {MinPrice:C}");
        }
    }
    
    private static void ValidateQuantity(Int32 quantity)
    {
        if (quantity <= 0)
        {
            throw new OrderValidationException("Quantity must be greater than zero");
        }
        
        if (quantity > MaxQuantity)
        {
            throw new OrderValidationException($"Quantity cannot exceed {MaxQuantity}");
        }
    }
    
    private static void ValidatePositiveQuantity(Int32 quantity)
    {
        if (quantity <= 0)
        {
            throw new OrderValidationException("Quantity must be greater than zero");
        }
    }
}
