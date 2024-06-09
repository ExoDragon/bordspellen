using Core.Domain.Data.Entities;
using Core.Domain.Dto.Entities;
using Core.Repositories.Data.Entities;

namespace ApplicationService.Data.Entities;

public interface IReviewApplicationService : IApplicationService<Review, ReviewDto>, IReviewRepository
{
    
}