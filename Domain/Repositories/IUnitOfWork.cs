namespace Domain.Repositories;

public interface IUnitOfWork : IDisposable
{
    Task<Int32> CommitAsync();
}