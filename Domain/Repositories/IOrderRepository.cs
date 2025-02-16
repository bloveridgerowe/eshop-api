using Domain.Aggregates.Orders;

namespace Domain.Repositories;

public interface IOrderRepository
{
    Task<List<Order>> GetOrdersForCustomerAsync(Guid customerId);
    Task<List<Order>> GetPendingOrdersAsync();
    Task<List<Order>> GetShippedOrdersAsync();
    Task<Order?> FindByIdAsync(Guid id);
    Task SaveAsync(Order order);
}