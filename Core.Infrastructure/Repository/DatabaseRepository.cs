using System.Linq.Expressions;
using Core.Domain;
using Core.Domain.Dto;
using Core.Infrastructure.Context;

namespace Core.Infrastructure.Repository;

public abstract class DatabaseRepository<T, TDto> : IDatabaseRepository<T, TDto> where T : Entity where TDto : DtoBase
{
    protected readonly EntityDatabaseContext DbContext;

    protected DatabaseRepository(EntityDatabaseContext dbContext)
    {
        DbContext = dbContext;
    }
    
    public abstract Task<T> Create(T entity);
    public abstract Task<T> Create(TDto entity);
    public abstract Task<IEnumerable<T>> GetAll(params Expression<Func<T, object>>[] expressions);
    public abstract Task<IEnumerable<T>> GetAll();
    public abstract Task<T?> GetById(int id, params Expression<Func<T, object>>[] expressions);
    public abstract Task<T?> GetById(int id);
    public abstract Task<T> Update(T entity);
    public abstract Task<T> Update(TDto entity);
    public abstract Task<T> Delete(T entity);
    public abstract Task<T?> Delete(int id);
}