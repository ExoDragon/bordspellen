using Core.Domain.Data.Entities;
using Core.Infrastructure.Context;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Infrastructure.Seeder;

public static class GameEventSeeder
{
    public static async Task Seed(IApplicationBuilder applicationBuilder)
    {
        using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
        {
            var context = serviceScope.ServiceProvider.GetService<EntityDatabaseContext>();
            if (context == null) return;

            await context.Database.EnsureCreatedAsync();

            if (!context.GameEvents.Any())
            {
                context.GameEvents.AddRange(new List<GameEvent>
                {
                    new()
                    {
                        Name = "First Cool Game Event",
                        Description = "This is the first cool game event",
                        Street = "Vogelven",
                        HouseNumber = "16",
                        PostalCode = "4631MP",
                        City = "Hoogerheide",
                        IsAdultEvent = false,
                        EventDate = new DateTime(2022, 10, 19, 18,00,00),
                        MaxAmountOfPlayers = 10,
                        OrganiserId = 1
                    },
                    new()
                    {
                        Name = "Another Cool Game Event",
                        Description = "This is another cool game event",
                        Street = "Bospad",
                        HouseNumber = "45",
                        PostalCode = "4645KH",
                        City = "Bergen",
                        IsAdultEvent = true,
                        EventDate = new DateTime(2022, 11, 8, 19,30,00),
                        MaxAmountOfPlayers = 15,
                        OrganiserId = 2
                    },
                    new()
                    {
                        Name = "Magic Tournament",
                        Description = "Organising a magic the gathering tournament",
                        Street = "Vogelven",
                        HouseNumber = "16",
                        PostalCode = "4631MP",
                        City = "Hoogerheide",
                        IsAdultEvent = true,
                        EventDate = new DateTime(2022, 11, 8, 22, 00, 00),
                        MaxAmountOfPlayers = 15,
                        OrganiserId = 1
                    }
                });
                await context.SaveChangesAsync();
            }
        }
    }
}