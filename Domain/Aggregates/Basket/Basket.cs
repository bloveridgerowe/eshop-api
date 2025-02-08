using Domain.Entities;
using Domain.Exceptions.Baskets;

namespace Domain.Aggregates.Basket;

public class Basket : Entity
{
    private const Int32 MaxItems = 50;
    
    public Guid CustomerId { get; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }
    public List<BasketItem> Items { get; } = [];
    public Decimal TotalPrice => Items.Sum(item => item.TotalPrice);

    public Basket(Guid id, Guid customerId, DateTime? createdAt = null, DateTime? updatedAt = null)
        : base(id)
    {
        ValidateCustomerId(customerId);
        
        DateTime utcNow = DateTime.UtcNow;
        
        CustomerId = customerId;
        CreatedAt = createdAt ?? utcNow;
        UpdatedAt = updatedAt ?? utcNow;
        
        ValidateDates(CreatedAt, UpdatedAt);
    }

    public void AddItem(BasketItem item)
    {
        ValidateItem(item);
        ValidateBasketNotFull();
        
        BasketItem? existingItem = Items.SingleOrDefault(i => i.ProductId == item.ProductId);

        if (existingItem is not null)
        {
            existingItem.IncreaseQuantity(item.Quantity);
        }
        else
        {
            Items.Add(item);
        }

        UpdatedAt = DateTime.UtcNow;
    }
    
    public void AddItems(IEnumerable<BasketItem> items)
    {
        ArgumentNullException.ThrowIfNull(items);
        
        List<BasketItem> itemsList = items.ToList();
        if (!itemsList.Any())
        {
            throw new BasketValidationException("Items collection cannot be empty");
        }
        
        if (Items.Count + itemsList.Count > MaxItems)
        {
            throw new BasketValidationException($"Cannot add {itemsList.Count} items. Basket cannot contain more than {MaxItems} items");
        }
        
        foreach (BasketItem item in itemsList)
        {
            ValidateItem(item);
        }
        
        foreach (BasketItem item in itemsList)
        {
            AddItem(item);
        }
    }

    public void RemoveItem(Guid productId)
    {
        ValidateProductId(productId);
        
        BasketItem? existingItem = Items.SingleOrDefault(i => i.ProductId == productId);

        if (existingItem is not null)
        {
            Items.Remove(existingItem);
            UpdatedAt = DateTime.UtcNow;
        }
        else
        {
            throw new BasketValidationException($"Product with ID {productId} not found in basket");
        }
    }

    public void UpdateItemQuantity(Guid productId, Int32 quantity)
    {
        ValidateProductId(productId);
        
        BasketItem? existingItem = Items.SingleOrDefault(i => i.ProductId == productId);

        if (existingItem is not null)
        {
            existingItem.UpdateQuantity(quantity);
            UpdatedAt = DateTime.UtcNow;
        }
        else
        {
            throw new BasketValidationException($"Product with ID {productId} not found in basket");
        }
    }

    public void UpdateItemPrice(Guid productId, Decimal price)
    {
        ValidateProductId(productId);
        
        BasketItem? existingItem = Items.SingleOrDefault(i => i.ProductId == productId);

        if (existingItem is not null)
        {
            existingItem.UpdatePrice(price);
            UpdatedAt = DateTime.UtcNow;
        }
        else
        {
            throw new BasketValidationException($"Product with ID {productId} not found in basket");
        }
    }

    public void ClearItems()
    {
        Items.Clear();
        UpdatedAt = DateTime.UtcNow;
    }
    
    private static void ValidateCustomerId(Guid customerId)
    {
        if (customerId == Guid.Empty)
        {
            throw new BasketValidationException("Customer ID cannot be empty");
        }
    }
    
    private static void ValidateProductId(Guid productId)
    {
        if (productId == Guid.Empty)
        {
            throw new BasketValidationException("Product ID cannot be empty");
        }
    }
    
    private static void ValidateItem(BasketItem item)
    {
        ArgumentNullException.ThrowIfNull(item);
    }
    
    private void ValidateBasketNotFull()
    {
        if (Items.Count >= MaxItems)
        {
            throw new BasketValidationException($"Basket cannot contain more than {MaxItems} items");
        }
    }
    
    private static void ValidateDates(DateTime createdAt, DateTime updatedAt)
    {
        if (createdAt > updatedAt)
        {
            throw new BasketValidationException("Created date cannot be later than updated date");
        }
    }
}