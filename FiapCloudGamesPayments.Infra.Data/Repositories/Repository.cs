using FiapCloudGamesPayments.Domain.Entities;
using FiapCloudGamesPayments.Domain.Repositories;
using FiapCloudGamesPayments.Infra.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace FiapCloudGamesPayments.Infra.Data.Repositories;

public class Repository<T> : IRepository<T> where T : EntityBase
{
    private readonly DbSet<T> _dbSet;

    public Repository(ContextDb contextDb)
    {
        _dbSet = contextDb.Set<T>();
    }

    public async Task AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
    }

    public void Delete(T entity)
    {
        _dbSet.Remove(entity);
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _dbSet.AsNoTracking().ToListAsync();
    }

    public async Task<T?> GetByIdAsync(Guid id)
    {
        return await _dbSet.FindAsync(id);
    }

    public void Update(T entity)
    {
        _dbSet.Update(entity);
    }

    public void Attach(T entity) => _dbSet.Attach(entity);
}
