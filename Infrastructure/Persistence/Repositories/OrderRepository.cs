using Domain.Aggregates.Orders;
using Domain.Repositories;
using Infrastructure.Persistence.Entities.Order;
using Infrastructure.Persistence.Mappers;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly ECommerceDbContext _context;

    public OrderRepository(ECommerceDbContext context)
    {
        _context = context;
    }

    public async Task<List<Order>> GetOrdersForCustomerAsync(Guid customerId)
    {
        return await _context.Orders
            .Include(o => o.Items)
            .ThenInclude(i => i.Product)
            .Where(o => o.CustomerId == customerId)
            .Select(o => o.ToDomain())
            .ToListAsync();
    }

    public async Task<List<Order>> GetPendingOrdersAsync()
    {
        return await _context.Orders
            .Include(o => o.Items)
            .ThenInclude(i => i.Product)
            .Where(o => o.Status.Id == OrderProcessingStatus.Pending.Id)
            .Select(o => o.ToDomain())
            .ToListAsync();
    }

    public async Task<List<Order>> GetShippedOrdersAsync()
    {
        return await _context.Orders
            .Include(o => o.Items)
            .ThenInclude(i => i.Product)
            .Where(o => o.Status.Id == OrderProcessingStatus.Shipped.Id)
            .Select(o => o.ToDomain())
            .ToListAsync();    
    }

    public async Task<List<Order>> FindByCustomerIdAsync(Guid id)
    {
        return await _context.Orders
            .Include(o => o.Items)
            .ThenInclude(i => i.Product)
            .Where(o => o.CustomerId == id)
            .Select(o => o.ToDomain())
            .ToListAsync();
    }

    public async Task<Order?> FindByIdAsync(Guid id)
    {
        OrderEntity? entity = await _context.Orders
            .Include(o => o.Items)
            .ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(o => o.Id == id);

        if (entity is null)
        {
            return null;
        }

        return entity.ToDomain();
    }

    public async Task SaveAsync(Order order)
    {
        // Fetch the existing order from the database
        OrderEntity? existingEntity = await _context.Orders
            .Include(o => o.Items)
            .ThenInclude(i => i.Product)
            .SingleOrDefaultAsync(o => o.Id == order.Id);

        if (existingEntity is null)
        {
            // If the order does not exist, add it as a new entity
            await _context.Orders.AddAsync(order.ToPersistence());
        }
        else
        {
            // Update the existing order properties
            existingEntity.CustomerId = order.CustomerId;
            existingEntity.StatusId = order.Status.Id;

            // Update or add order items
            foreach (OrderItem domainItem in order.Items)
            {
                OrderItemEntity? existingItem = existingEntity.Items
                    .SingleOrDefault(i => i.ProductId == domainItem.ProductId);

                if (existingItem is not null)
                {
                    // Update existing item
                    existingItem.Price = domainItem.Price;
                    existingItem.Quantity = domainItem.Quantity;
                }
                else
                {
                    // Add new item
                    await _context.OrderItems.AddAsync(domainItem.ToPersistence(order.Id));
                }
            }

            // Remove items that are no longer in the domain model
            List<OrderItemEntity> itemsToRemove = existingEntity.Items
                .Where(existingItem => !order.Items.Any(domainItem => domainItem.ProductId == existingItem.ProductId))
                .ToList();

            foreach (OrderItemEntity itemToRemove in itemsToRemove)
            {
                _context.OrderItems.Remove(itemToRemove);
            }
        }

        // Changes saved using ECommerceUnitOfWork
    }
}