namespace Domain.Exceptions.Orders;

public class PriceChangeException : Exception
{
    public Guid ProductId { get; }
    public Decimal BasketPrice { get; }
    public Decimal ProductPrice { get; }

    public PriceChangeException(Guid productId, Decimal basketPrice, Decimal productPrice)
        : base($"The price of the product {productId} has changed from {basketPrice} to {productPrice}.")
    {
        ProductId = productId;
        BasketPrice = basketPrice;
        ProductPrice = productPrice;
    }
} 