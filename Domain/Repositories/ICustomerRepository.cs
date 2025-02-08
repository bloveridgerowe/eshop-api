using Domain.Entities;

namespace Domain.Repositories;

public interface ICustomerRepository
{
    Task<Customer?> FindByIdAsync(Guid id);
    Task SaveAsync(Customer customer);
}