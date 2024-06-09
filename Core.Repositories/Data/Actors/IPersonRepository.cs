using Core.Domain.Data.Actors;
using Core.Domain.Dto.Actors;
using Core.DomainService.Data.Actors;

namespace Core.Repositories.Data.Actors;

public interface IPersonRepository : IRepository<Person, PersonDto>, IPersonDomainService
{
    
}