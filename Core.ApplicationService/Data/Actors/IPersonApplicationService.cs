using Core.Domain.Data.Actors;
using Core.Domain.Dto.Actors;
using Core.Repositories.Data.Actors;

namespace ApplicationService.Data.Actors;

public interface IPersonApplicationService : IApplicationService<Person, PersonDto>, IPersonRepository
{
    
}