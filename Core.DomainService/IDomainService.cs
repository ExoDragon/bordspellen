using System.Linq.Expressions;
using Core.Domain;
using Core.Domain.Dto;

namespace Core.DomainService;

public interface IDomainService<T, TDto> where T : Entity where TDto : DtoBase
{
    public Task<IEnumerable<T>> GetAll(params Expression<Func<T, object>>[] expressions);
    public Task<IEnumerable<T>> GetAll();
    public Task<T?> GetById(int id, params Expression<Func<T, object>>[] expressions);
    public Task<T?> GetById(int id);
    public Task<T> Create(T entity);
    public Task<T> Create(TDto entity);
    
    public Task<T> Update(T entity);
    public Task<T> Update(TDto entity);
    
    public Task<T> Delete(T entity);
    public Task<T?> Delete(int id);
}