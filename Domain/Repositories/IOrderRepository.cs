using Domain.Aggregates.Orders;

namespace Domain.Repositories;

public interface IOrderRepository
{
    Task<List<Order>> GetAllAsync(Guid customerId);
    Task<List<Order>> FindByCustomerIdAsync(Guid id);
    Task<Order?> FindByIdAsync(Guid id);
    Task SaveAsync(Order order);
}