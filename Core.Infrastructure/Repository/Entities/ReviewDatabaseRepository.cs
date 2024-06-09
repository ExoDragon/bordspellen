using Core.Domain.Data.Actors;
using Core.Domain.Data.Entities;
using Core.Domain.Dto.Entities;
using Core.Infrastructure.Context;
using Core.Infrastructure.Repository.Generic;
using Core.Repositories.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Core.Infrastructure.Repository.Entities;

public class ReviewDatabaseRepository : GenericDatabaseRepository<Review, ReviewDto>, IReviewRepository
{
    public ReviewDatabaseRepository(EntityDatabaseContext dbContext) : base(dbContext)
    {
    }
    
    public async Task<Review?> CreateReview(ReviewDto entity, Person person, GameEvent? gameEvent)
    {
        var personEntity = await DbContext.Persons.FirstOrDefaultAsync(p => p.Id == person.Id);
        var gameEventEntity = await DbContext.GameEvents.FirstOrDefaultAsync(g => g.Id == gameEvent!.Id);

        if (personEntity == null || gameEventEntity == null)
        {
            return null;
        }

        var review = new Review
        {
            EventId = gameEventEntity.Id,
            ReviewPosterId = personEntity.Id,
            Rating = entity.Rating,
            ReviewDescription = entity.ReviewDescription
        };
        
        var newReview = await Create(review);
        return newReview;
    }
    public async Task<Review?> UpdateReview(ReviewDto entity, Person person, GameEvent gameEvent)
    {
        var personEntity = await DbContext.Persons.FirstOrDefaultAsync(p => p.Id == person.Id);
        var gameEventEntity = await DbContext.GameEvents.FirstOrDefaultAsync(g => g.Id == gameEvent.Id);
        var reviewEntity = await DbContext.Reviews.FirstOrDefaultAsync(r => r.Id == entity.Id);

        var id = entity.Id;
        
        if (personEntity == null || 
            gameEventEntity == null || 
            reviewEntity == null)
        {
            return null;
        }

        reviewEntity.Rating = entity.Rating;
        reviewEntity.ReviewDescription = entity.ReviewDescription;
        await DbContext.SaveChangesAsync();

        return reviewEntity;
    }
    public async Task<Review?> GetEventTopRatedReview(GameEvent gameEvent)
    {
        List<Review> reviews = await DbContext.Set<Review>().Where(r => r.GameEvent!.Id == gameEvent.Id).ToListAsync();
        return reviews.Count == 0 ? null : reviews.MaxBy(r => r.Rating);
    }
    public async Task<int> GetAverageOrganiserRating(Person person)
    {
        var gameEvents = await DbContext.GameEvents.Where(g => g.OrganiserId == person.Id)
            .Include(g => g.ReviewsRecieved)
            .ToListAsync();

        if (gameEvents.Count == 0)
        {
            return 0;
        }

        int devider = 0;
        int sum = 0;
        
        foreach (var gameEvent in gameEvents.Where(gameEvent => gameEvent.ReviewsRecieved!.Count != 0))
        {
            var tempdevider = 0;
            var tempsum = 0;
            
            foreach (var review in gameEvent.ReviewsRecieved!)
            {
                tempsum += review.Rating;
                tempdevider++;
            }
            sum += tempsum / tempdevider;
            devider++;
        }
        return devider == 0 ? sum : sum / devider; 
    }
}