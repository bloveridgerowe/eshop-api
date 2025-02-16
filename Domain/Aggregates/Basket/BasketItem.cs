using Domain.Exceptions.Baskets;

namespace Domain.Aggregates.Basket;

public class BasketItem
{
    private const Int32 MaxQuantity = 99;
    private const Decimal MinPrice = 0.01m;
    
    public Guid ProductId { get; }
    public String Name { get; }
    public Int32 Quantity { get; private set; }
    public Decimal Price { get; private set; }
    public Decimal TotalPrice => Price * Quantity;

    public BasketItem(Guid productId, Int32 quantity, Decimal price, String name)
    {
        ValidateProductId(productId);
        ValidateQuantity(quantity);
        ValidatePrice(price);
        
        ProductId = productId;
        Quantity = quantity;
        Price = price;
        Name = name;
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
            throw new BasketItemValidationException("Product ID cannot be empty");
        }
    }
    
    private static void ValidateQuantity(Int32 quantity)
    {
        if (quantity <= 0)
        {
            throw new BasketItemValidationException("Quantity must be greater than zero");
        }
        
        if (quantity > MaxQuantity)
        {
            throw new BasketItemValidationException($"Quantity cannot exceed {MaxQuantity}");
        }
    }
    
    private static void ValidatePositiveQuantity(Int32 quantity)
    {
        if (quantity <= 0)
        {
            throw new BasketItemValidationException("Quantity must be greater than zero");
        }
    }
    
    private static void ValidatePrice(Decimal price)
    {
        if (price < MinPrice)
        {
            throw new BasketItemValidationException($"Price must be at least {MinPrice:C}");
        }
    }
}
