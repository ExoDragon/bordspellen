using Core.Domain.Data.Actors;
using Core.Domain.Enums;
using Core.Infrastructure.Context;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Infrastructure.Seeder;

public static class PersonSeeder
{
    public static async Task Seed(IApplicationBuilder applicationBuilder)
    {
        using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
        {
            var context = serviceScope.ServiceProvider.GetService<EntityDatabaseContext>();
            if (context == null) return;

            await context.Database.EnsureCreatedAsync();

            if (!context.Persons.Any())
            {
                context.Persons.AddRange(new List<Person>
                {
                    new()
                    {
                        FirstName = "Thomas",
                        LastName = "Terlaak",
                        Email = "to.terlaak@outlook.com",
                        Gender = GenderEnum.MALE,
                        DateOfBirth = new DateTime(1999, 11,8),
                        Street = "Vogelven",
                        HouseNumber = "16",
                        PostalCode = "4631MP",
                        City = "Hoogerheide",
                        PhoneNumber = "06-123456789"
                    },
                    new()
                    {
                        FirstName = "Thomas",
                        LastName = "Anders",
                        Email = "to.anders@outlook.com",
                        Gender = GenderEnum.MALE,
                        DateOfBirth = new DateTime(2006, 11,8),
                        Street = "Vogelven",
                        HouseNumber = "16",
                        PostalCode = "4631MP",
                        City = "Hoogerheide",
                        PhoneNumber = "06-123456789"
                    },
                    new()
                    {
                        FirstName = "Wouter",
                        LastName = "Terlaak",
                        Email = "wa.terlaak@outlook.com",
                        Gender = GenderEnum.MALE,
                        DateOfBirth = new DateTime(2006, 9,8),
                        Street = "Vogelven",
                        HouseNumber = "16",
                        PostalCode = "4631MP",
                        City = "Hoogerheide",
                        PhoneNumber = "06-123456789"
                    },
                    new()
                    {
                        FirstName = "Jantje",
                        LastName = "Kantje",
                        Email = "ja.kantje@live.com",
                        Gender = GenderEnum.MALE,
                        DateOfBirth = new DateTime(1999, 1,30),
                        Street = "Hogestraat",
                        HouseNumber = "109",
                        PostalCode = "5432KB",
                        City = "Opperdoes",
                        PhoneNumber = "06-123456789"
                    }
                });
                await context.SaveChangesAsync();
            }
        }
    }
}