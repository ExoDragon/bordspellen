using Core.Domain.Data.Entities;
using Core.Domain.Dto.Entities;
using Core.Infrastructure.Context;
using Core.Infrastructure.Repository.Generic;
using Core.Repositories.Data.Entities;

namespace Core.Infrastructure.Repository.Entities;

public class BoardGameDatabaseRepository : GenericDatabaseRepository<BoardGame, BoardGameDto>, IBoardGameRepository
{
    public BoardGameDatabaseRepository(EntityDatabaseContext dbContext) : base(dbContext)
    {
    }
}