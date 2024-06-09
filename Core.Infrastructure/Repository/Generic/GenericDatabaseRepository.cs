using System.Linq.Expressions;
using Core.Domain;
using Core.Domain.Dto;
using Core.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Core.Infrastructure.Repository.Generic;

public abstract class GenericDatabaseRepository<T, TDto> : DatabaseRepository<T, TDto> where T : Entity where TDto : DtoBase
{
    protected GenericDatabaseRepository(EntityDatabaseContext dbContext) : base(dbContext)
    {
    }
    public override async Task<IEnumerable<T>> GetAll()
    {
        return await DbContext.Set<T>().ToListAsync();
    }
    public override Task<T> Create(TDto entity)
    {
        throw new NotImplementedException();
    }
    public override async Task<IEnumerable<T>> GetAll(params Expression<Func<T, object>>[] expressions)
    {
        IQueryable<T> query = DbContext.Set<T>();
        query = expressions.Aggregate(query, (current, expression) => current.Include(expression));
        return await query.ToListAsync();
    }
    public override async Task<T?> GetById(int id)
    {
        return await DbContext.Set<T>().FirstOrDefaultAsync(t => t.Id == id);
    }
    public override async Task<T?> GetById(int id, params Expression<Func<T, object>>[] expressions)
    {
        IQueryable<T?> query = DbContext.Set<T>();
#pragma warning disable CS8634
        query = expressions.Aggregate(query, (current, expression) => current.Include(expression!));
#pragma warning restore CS8634
        return await query.FirstOrDefaultAsync(t => t!.Id == id);
    }
    public override async Task<T> Create(T entity)
    {
        var newEntity = await DbContext.Set<T>().AddAsync(entity);
        await DbContext.SaveChangesAsync();
        return newEntity.Entity;
    }
    public override async Task<T> Update(T entity)
    {
        EntityEntry entityEntry = DbContext.Update(entity);
        await DbContext.SaveChangesAsync();
        return (T) entityEntry.Entity;
    }
    public override Task<T> Update(TDto entity)
    {
        throw new NotImplementedException();
    }
    public override async Task<T> Delete(T entity)
    {
        EntityEntry entityEntry = DbContext.Entry(entity);
        entityEntry.State = EntityState.Deleted;
            
        await DbContext.SaveChangesAsync();
        return entity;
    }
    public override async Task<T?> Delete(int id)
    {
        var entity = await DbContext.Set<T>().FirstOrDefaultAsync(t => t.Id == id);
        if (entity == null) return null;
        
        EntityEntry entityEntry = DbContext.Entry(entity);
        entityEntry.State = EntityState.Deleted;
            
        await DbContext.SaveChangesAsync();
        return entity;
    }
}