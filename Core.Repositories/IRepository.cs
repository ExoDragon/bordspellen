using Core.Domain;
using Core.Domain.Dto;
using Core.DomainService;

namespace Core.Repositories;

public interface IRepository<T, TDto> : IDomainService<T, TDto> where T : Entity where TDto : DtoBase
{
    
}