using Core.Domain.Data.Link;
using Core.Infrastructure.Context;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Infrastructure.Seeder;

public static class PersonDietsSeeder
{
    public static async Task Seed(IApplicationBuilder applicationBuilder)
    {
        using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
        {
            var context = serviceScope.ServiceProvider.GetService<EntityDatabaseContext>();
            if (context == null) return;

            await context.Database.EnsureCreatedAsync();

            if (!context.PersonDiets.Any())
            {
                context.PersonDiets.AddRange(new List<PersonDiets>
                {
                    new ()
                    {
                        DietId = 4,
                        PersonId = 1
                    },
                    new ()
                    {
                        DietId = 2,
                        PersonId = 1
                    },
                    new ()
                    {
                        DietId = 2,
                        PersonId = 2
                    },
                    new ()
                    {
                        DietId = 1,
                        PersonId = 3
                    }
                });
                await context.SaveChangesAsync();
            }
        }
    }
}