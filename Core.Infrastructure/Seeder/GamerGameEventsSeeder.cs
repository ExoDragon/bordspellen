using Core.Domain.Data.Link;
using Core.Infrastructure.Context;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Infrastructure.Seeder;

public static class PersonGameEventsSeeder
{
    public static async Task Seed(IApplicationBuilder applicationBuilder)
    {
        using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
        {
            var context = serviceScope.ServiceProvider.GetService<EntityDatabaseContext>();
            if (context == null) return;

            await context.Database.EnsureCreatedAsync();

            if (!context.PersonGameEvents.Any())
            {
                context.PersonGameEvents.AddRange(new List<PersonGameEvents>
                {
                    new ()
                    {
                        PersonId = 1,
                        GameEventId = 1
                    },
                    new ()
                    {
                        PersonId = 2,
                        GameEventId = 1
                    },
                    new ()
                    {
                        PersonId = 3,
                        GameEventId = 1
                    },
                    new ()
                    {
                        PersonId = 1,
                        GameEventId = 2
                    },
                    new ()
                    {
                        PersonId = 3,
                        GameEventId = 2
                    }
                });
                await context.SaveChangesAsync();
            }
        }
    }
}