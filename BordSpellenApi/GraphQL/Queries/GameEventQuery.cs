using Core.Domain.Data.Entities;
using Core.Repositories.Data.Entities;

namespace BordSpellenApi.GraphQL.Queries;

[ExtendObjectType(typeof(BaseQuery))]
public class GameEventQuery
{
    [UseFiltering]
    [UseSorting]
    public async Task<IEnumerable<GameEvent>> GetGameEvents([Service] IGameEventRepository gameEventRepository)
    {
        return await gameEventRepository.GetAll(
            r => r.Organiser!,
            r => r.ReviewsRecieved!,
            r => r.GamerGameEvents!,
            r => r.AvailableFoodTypes!,
            r => r.BoardGameEvents!);
    }
    
    public async Task<GameEvent?> GetGameEventById(int id, [Service] IGameEventRepository gameEventRepository)
    {
        return await gameEventRepository.GetById(id);
    }
}