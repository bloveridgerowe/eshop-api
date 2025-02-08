using Domain.Aggregates.Basket;

namespace Domain.Repositories;

public interface IBasketRepository
{
    public Task SaveAsync(Basket basket);
    public Task<Basket?> FindByIdAsync(Guid basketId);
    public Task<Basket?> FindByCustomerIdAsync(Guid customerId);
}