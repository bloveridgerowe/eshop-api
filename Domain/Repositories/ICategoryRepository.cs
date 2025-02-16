using Domain.Entities;

namespace Domain.Repositories;

public interface ICategoryRepository
{
    Task<List<Category>> GetAllCategoriesAsync();
    Task SaveAsync(Category category);
}