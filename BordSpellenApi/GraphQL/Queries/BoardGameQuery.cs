using Core.Domain.Data.Entities;
using Core.Repositories.Data.Entities;
using HotChocolate.AspNetCore.Authorization;

namespace BordSpellenApi.GraphQL.Queries;

[ExtendObjectType(typeof(BaseQuery))]
public class BoardGameQuery
{
    [UseFiltering]
    [UseSorting]
    public async Task<IEnumerable<BoardGame>> GetBoardGames([Service] IBoardGameRepository boardGameRepository)
    {
        return await boardGameRepository.GetAll();
    }
    
    public async Task<BoardGame?> GetBoardGameById(int id, [Service] IBoardGameRepository boardGameRepository)
    {
        return await boardGameRepository.GetById(id);
    }
}