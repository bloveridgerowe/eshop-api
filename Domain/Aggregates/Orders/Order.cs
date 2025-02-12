using Domain.Entities;
using Domain.Events.Orders;
using Domain.Exceptions.Orders;

namespace Domain.Aggregates.Orders;

public class Order : Entity
{
    private const Int32 MaxItems = 99;
    private const Decimal MinOrderAmount = 0.01m;
    
    public Guid CustomerId { get; }
    public List<OrderItem> Items { get; } = [];
    public Decimal TotalPrice => Items.Sum(item => item.TotalPrice);  
    public DateTime CreatedAt { get; }
    
    public OrderProcessingStatus Status { get; private set; }
    
    public Order(Guid id, Guid customerId)
        : this(id, customerId, DateTime.UtcNow)
    {
    }

    public Order(Guid id, Guid customerId, DateTime createdAt)
        : base(id)
    {
        ValidateId(id);
        ValidateCustomerId(customerId);
        
        CustomerId = customerId;
        Status = OrderProcessingStatus.Pending;
        CreatedAt = createdAt;
    }
    
    internal Order(Guid id, Guid customerId, OrderProcessingStatus status)
        : base(id)
    {
        ValidateId(id);
        ValidateCustomerId(customerId);
        
        CustomerId = customerId;
        Status = status;
        CreatedAt = DateTime.UtcNow;
        
        AddEvent(new OrderStatusChangedEvent(Id, status));
    }

    public void AddItem(OrderItem item)
    {
        ValidateOrderItem(item);
        ValidateCanModifyOrder();
        
        OrderItem? existingItem = Items.SingleOrDefault(i => i.ProductId == item.ProductId);

        if (existingItem is not null)
        {
            existingItem.IncreaseQuantity(item.Quantity);
        }
        else
        {
            if (Items.Count >= MaxItems)
            {
                throw new OrderValidationException($"Order cannot have more than {MaxItems} unique items");
            }
            
            Items.Add(item);
        }
    }
    
    public void AddItems(IEnumerable<OrderItem> items)
    {
        ArgumentNullException.ThrowIfNull(items);
        
        foreach (OrderItem item in items)
        {
            AddItem(item);
        }
    }

    public void RemoveItem(Guid productId)
    {
        ValidateCanModifyOrder();
        
        if (productId == Guid.Empty)
        {
            throw new OrderValidationException("Product ID cannot be empty");
        }
        
        OrderItem? existingItem = Items.SingleOrDefault(i => i.ProductId == productId);

        if (existingItem is not null)
        {
            Items.Remove(existingItem);
        }
    }

    public void SetStatus(OrderProcessingStatus status)
    {
        if (status == OrderProcessingStatus.Pending)
        {
            return;
        }

        if (status == OrderProcessingStatus.Shipped)
        {
            MarkAsShipped();
            return;
        }

        if (status == OrderProcessingStatus.Delivered)
        {
            MarkAsDelivered();
            return;
        }

        if (status == OrderProcessingStatus.Canceled)
        {
            MarkAsCancelled();
        }
    }

    public void MarkAsShipped()
    {
        if (Status != OrderProcessingStatus.Pending)
        {
            throw new OrderStatusException(
                "Only pending orders can be marked as shipped",
                Status,
                OrderProcessingStatus.Shipped);
        }

        if (!Items.Any())
        {
            throw new OrderValidationException("Cannot ship an empty order");
        }

        Status = OrderProcessingStatus.Shipped;
        
        AddEvent(new OrderStatusChangedEvent(Id, OrderProcessingStatus.Shipped));
    }

    public void MarkAsDelivered()
    {
        Status = OrderProcessingStatus.Delivered;
        
        AddEvent(new OrderStatusChangedEvent(Id, OrderProcessingStatus.Delivered));
    }
    
    public void MarkAsCancelled()
    {
        if (Status != OrderProcessingStatus.Pending)
        {
            throw new OrderStatusException(
                "Only pending orders can be canceled",
                Status,
                OrderProcessingStatus.Canceled);
        }

        Status = OrderProcessingStatus.Canceled;
        
        AddEvent(new OrderStatusChangedEvent(Id, OrderProcessingStatus.Canceled));
    }
    
    private static void ValidateId(Guid id)
    {
        if (id == Guid.Empty)
        {
            throw new OrderValidationException("Order ID cannot be empty");
        }
    }
    
    private static void ValidateCustomerId(Guid customerId)
    {
        if (customerId == Guid.Empty)
        {
            throw new OrderValidationException("Customer ID cannot be empty");
        }
    }
    
    private static void ValidateOrderItem(OrderItem item)
    {
        ArgumentNullException.ThrowIfNull(item);
    }
    
    private void ValidateCanModifyOrder()
    {
        if (Status != OrderProcessingStatus.Pending)
        {
            throw new OrderValidationException("Can only modify pending orders");
        }
    }
}
