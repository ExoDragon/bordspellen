using Core.Domain.Data.Entities;
using Core.Infrastructure.Context;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Infrastructure.Seeder;

public class DietSeeder
{
    public static async Task Seed(IApplicationBuilder applicationBuilder)
    {
        using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
        {
            var context = serviceScope.ServiceProvider.GetService<EntityDatabaseContext>();
            if (context == null) return;

            await context.Database.EnsureCreatedAsync();

            if (!context.Diets.Any())
            {
                context.Diets.AddRange(new List<Diet>
                {
                   new ()
                   {
                       Name = "Lactose Intolerant",
                       Description = "Cannot consume dairy products"
                   },
                   new ()
                   {
                       Name = "Gluten Allergy",
                       Description = "Cannot consume products with gluten in it"
                   },
                   new ()
                   {
                       Name = "Vegetarian",
                       Description = "Cannot consume meat or dairy products"
                   },
                   new ()
                   {
                       Name = "Vegan",
                       Description = "Cannot consume animal products"
                   },
                   new ()
                   {
                       Name = "Non Alcoholic",
                       Description = "Cannot or won't Drink alcohol"
                   }
                });
                await context.SaveChangesAsync();
            }
        }
    }
}