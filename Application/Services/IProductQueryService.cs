using Application.DataTransferObjects;

namespace Application.Services;

// Sometimes our queries do not require a fully hydrated domain entity
// For these instances, we reduce load on the database by returning only required data
public interface IProductQueryService
{
    Task<List<ProductSummary>> FindFeaturedAsync();
    Task<List<ProductSummary>> FindByCategoryAsync(Guid categoryId);
    Task<List<ProductSummary>> FindByPartialNameAsync(String partialName);
}