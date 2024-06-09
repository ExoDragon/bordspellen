using Core.Domain;
using Core.Domain.Dto;
using Core.Repositories;

namespace Core.Infrastructure.Repository;

public interface IDatabaseRepository<T, TDto> : IRepository<T, TDto> where T : Entity where TDto : DtoBase
{
    
}