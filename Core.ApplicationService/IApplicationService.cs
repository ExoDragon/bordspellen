using Core.Domain;
using Core.Domain.Dto;
using Core.Repositories;

namespace ApplicationService;

public interface IApplicationService<T, TDto> : IRepository<T, TDto> where T : Entity where TDto : DtoBase
{
    
}