using Core.Domain.Data.Actors;
using Core.Domain.Data.Entities;
using Core.Domain.Dto.Entities;

namespace Core.DomainService.Data.Entities;

public interface IReviewDomainService : IDomainService<Review, ReviewDto>
{
    public Task<Review?> CreateReview(ReviewDto entity, Person person, GameEvent? gameEvent);
    public Task<Review?> UpdateReview(ReviewDto entity, Person person, GameEvent gameEvent);
    public Task<Review?> GetEventTopRatedReview(GameEvent gameEvent);
    public Task<int> GetAverageOrganiserRating(Person person);
}