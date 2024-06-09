using Core.Domain.Data.Entities;
using Core.Infrastructure.Context;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Infrastructure.Seeder;

public static class ReviewSeeder
{
    public static async Task Seed(IApplicationBuilder applicationBuilder)
    {
        using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
        {
            var context = serviceScope.ServiceProvider.GetService<EntityDatabaseContext>();
            if (context == null) return;

            await context.Database.EnsureCreatedAsync();

            if (!context.Reviews.Any())
            {
                context.Reviews.AddRange(new List<Review>
                {
                    new ()
                    {
                        Rating = 4,
                        ReviewDescription = "Great evening but, i had a run in with other people. Wish that could be resolved next time",
                        EventId = 1,
                        ReviewPosterId = 4
                    },
                    new ()
                    {
                        Rating = 3,
                        ReviewDescription = "Had a Great time. Next time we need to play Cards against Humanity",
                        EventId = 1,
                        ReviewPosterId = 3
                    },
                    new ()
                    {
                        Rating = 5,
                        ReviewDescription = "Had a Great time. Next time we need to play Cards against Humanity",
                        EventId = 1,
                        ReviewPosterId = 3
                    }
                });
                await context.SaveChangesAsync();
            }
        }
    }
}