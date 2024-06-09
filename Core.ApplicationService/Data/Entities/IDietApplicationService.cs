using Core.Domain.Data.Entities;
using Core.Domain.Dto.Entities;
using Core.Repositories.Data.Entities;

namespace ApplicationService.Data.Entities;

public interface IDietApplicationService : IApplicationService<Diet, DietDto>, IDietRepository
{
    
}