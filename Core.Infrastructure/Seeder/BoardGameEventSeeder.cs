using Core.Domain.Data.Link;
using Core.Infrastructure.Context;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Infrastructure.Seeder;

public static class BoardGameEventSeeder
{
    public static async Task Seed(IApplicationBuilder applicationBuilder)
    {
        using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
        {
            var context = serviceScope.ServiceProvider.GetService<EntityDatabaseContext>();
            if (context == null) return;

            await context.Database.EnsureCreatedAsync();

            if (!context.BoardGameEvents.Any())
            {
                context.BoardGameEvents.AddRange(new List<BoardGameEvents>
                {
                    //BoardGames for First GameEvent
                    new ()
                    {
                       BoardGameId = 1,
                       GameEventId = 1
                    },
                    new ()
                    {
                        BoardGameId = 5,
                        GameEventId = 1
                    },
                    new ()
                    {
                        BoardGameId = 4,
                        GameEventId = 1
                    },
                    
                    //BoardGames for Second GameEvent
                    new ()
                    {
                        BoardGameId = 2,
                        GameEventId = 2
                    },
                    new ()
                    {
                        BoardGameId = 3,
                        GameEventId = 2
                    },
                    
                    //BoardGames for Third GameEvent
                    new ()
                    {
                        BoardGameId = 3,
                        GameEventId = 3
                    }
                });
                await context.SaveChangesAsync();
            }
        }
    }
}