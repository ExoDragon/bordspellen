using Core.Domain.Data.Entities;
using Core.Domain.Dto.Entities;
using Core.Repositories.Data.Entities;

namespace ApplicationService.Data.Entities;

public interface IBoardGameApplicationService : IApplicationService<BoardGame, BoardGameDto>, IBoardGameRepository
{
    
}