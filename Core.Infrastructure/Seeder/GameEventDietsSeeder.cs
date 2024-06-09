using Core.Domain.Data.Link;
using Core.Infrastructure.Context;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Infrastructure.Seeder;

public static class GameEventDietsSeeder
{
    public static async Task Seed(IApplicationBuilder applicationBuilder)
    {
        using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
        {
            var context = serviceScope.ServiceProvider.GetService<EntityDatabaseContext>();
            if (context == null) return;

            await context.Database.EnsureCreatedAsync();

            if (!context.GameEventDiets.Any())
            {
                context.GameEventDiets.AddRange(new List<GameEventDiets>
                {
                    //Diets for First GameEvent
                    new ()
                    {
                        DietId = 1,
                        GameEventId = 1
                    },
                    new ()
                    {
                        DietId = 3,
                        GameEventId = 1
                    },
                    new ()
                    {
                        DietId = 4,
                        GameEventId = 1
                    },
                    
                    //Diets for Second GameEvent
                    new ()
                    {
                        DietId = 2,
                        GameEventId = 2
                    },
                    new ()
                    {
                        DietId = 3,
                        GameEventId = 2
                    }
                });
                await context.SaveChangesAsync();
            }
        }
    }
}