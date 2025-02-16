using Domain.Aggregates.Basket;
using Domain.Repositories;
using Infrastructure.Persistence.Entities.Basket;
using Infrastructure.Persistence.Mappers;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class BasketRepository : IBasketRepository
{
    private readonly ECommerceDbContext _context;

    public BasketRepository(ECommerceDbContext context)
    {
        _context = context;
    }

    public async Task<Basket?> FindByIdAsync(Guid basketId)
    {
        BasketEntity? entity = await _context.Baskets
            .Include(b => b.Items)
            .SingleOrDefaultAsync(b => b.Id == basketId);

        return entity?.ToDomain();
    }

    public async Task<Basket?> FindByCustomerIdAsync(Guid customerId)
    {
        BasketEntity? entity = await _context.Baskets
            .Include(b => b.Items)
            .ThenInclude(it => it.Product)
            .SingleOrDefaultAsync(b => b.CustomerId == customerId);
        
        return entity?.ToDomain();
    }

    public async Task SaveAsync(Basket basket)
    {
        // Fetch the existing basket from the database
        BasketEntity? existingEntity = await _context.Baskets
            .Include(b => b.Items)
            .SingleOrDefaultAsync(b => b.Id == basket.Id);

        if (existingEntity is null)
        {
            // If the basket does not exist, add it as a new entity
            await _context.Baskets.AddAsync(basket.ToPersistence());
        }
        else
        {
            // Update the existing basket properties
            existingEntity.CustomerId = basket.CustomerId;
            existingEntity.UpdatedAt = basket.UpdatedAt;

            // Update or add basket items
            foreach (BasketItem domainItem in basket.Items)
            {
                BasketItemEntity? existingItem = existingEntity.Items
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
                    _context.BasketItems.Add(domainItem.ToPersistence(basket.Id));
                }
            }

            // Remove items that are no longer in the domain model
            List<BasketItemEntity> itemsToRemove = existingEntity.Items
                .Where(existingItem => !basket.Items.Any(domainItem => domainItem.ProductId == existingItem.ProductId))
                .ToList();

            foreach (BasketItemEntity itemToRemove in itemsToRemove)
            {
                _context.BasketItems.Remove(itemToRemove);
            }
        }

        // Changes saved using ECommerceUnitOfWork
    }
}