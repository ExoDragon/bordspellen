using Core.Domain.Data.Entities;
using Core.Domain.Dto.Entities;
using Core.DomainService.Data.Entities;

namespace Core.Repositories.Data.Entities;

public interface IReviewRepository : IRepository<Review, ReviewDto>, IReviewDomainService
{
    
}