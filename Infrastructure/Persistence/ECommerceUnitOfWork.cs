using Domain.Repositories;

namespace Infrastructure.Persistence;

public class ECommerceUnitOfWork : IUnitOfWork
{
    private readonly ECommerceDbContext _context;

    public ECommerceUnitOfWork(ECommerceDbContext context)
    {
        _context = context;
    }

    public async Task<Int32> CommitAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}