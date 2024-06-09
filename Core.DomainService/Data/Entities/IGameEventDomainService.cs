using Core.Domain.Data.Actors;
using Core.Domain.Data.Entities;
using Core.Domain.Data.Link;
using Core.Domain.Dto.Entities;

namespace Core.DomainService.Data.Entities;

public interface IGameEventDomainService : IDomainService<GameEvent, GameEventDto>
{
    public Task<IEnumerable<GameEvent>> GetReviewedGameEvents(Person person);
    public Task<bool> Subscribe(GameEvent entity, Person person);
    public Task<bool> Unsubscribe(PersonGameEvents entity);
    public Task<IEnumerable<GameEvent>> GetSubscribedEvents(Person person);
    public Task<bool> CheckFoodAvailability(GameEvent entity, Person person);
    public Task<bool> CheckSubscription(GameEvent gameEvent, Person person);
}