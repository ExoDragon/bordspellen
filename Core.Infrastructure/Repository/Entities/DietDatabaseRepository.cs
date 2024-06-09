using Core.Domain.Data.Entities;
using Core.Domain.Dto.Entities;
using Core.Infrastructure.Context;
using Core.Infrastructure.Repository.Generic;
using Core.Repositories.Data.Entities;

namespace Core.Infrastructure.Repository.Entities;

public class DietDatabaseRepository : GenericDatabaseRepository<Diet, DietDto>, IDietRepository
{
    public DietDatabaseRepository(EntityDatabaseContext dbContext) : base(dbContext)
    {
    }
}