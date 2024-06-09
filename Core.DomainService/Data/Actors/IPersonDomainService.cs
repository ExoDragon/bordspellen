using Core.Domain.Data.Actors;
using Core.Domain.Dto.Actors;

namespace Core.DomainService.Data.Actors;

public interface IPersonDomainService : IDomainService<Person, PersonDto>
{
    public Task<Person?> GetOneByEmail(string email);
    public Task<Person?> GetOneWithDiet(string email);
}