using Core.Domain.Data.Actors;
using Core.Domain.Data.Link;
using Core.Domain.Dto.Actors;
using Core.Domain.Dto.Helper;
using Core.Infrastructure.Context;
using Core.Infrastructure.Repository.Generic;
using Core.Repositories.Data.Actors;
using Microsoft.EntityFrameworkCore;

namespace Core.Infrastructure.Repository.Actors;

public class PersonDatabaseRepository : GenericDatabaseRepository<Person, PersonDto>, IPersonRepository
{
    public PersonDatabaseRepository(EntityDatabaseContext dbContext) : base(dbContext)
    {
    }

    public async Task<Person?> GetOneByEmail(string email)
    {
        return await DbContext.Set<Person>().FirstOrDefaultAsync(p => p.Email == email);
    }
    public async Task<Person?> GetOneWithDiet(string email)
    {
        return await DbContext.Set<Person>().Include(x=> x.DietList!)
            .ThenInclude(x => x.Diet)
            .FirstOrDefaultAsync(p => p.Email == email);
    }
    public override async Task<Person> Create(PersonDto entity)
    {
        var transferPerson = entity.ReturnToDomainModel();
        var newPerson = await Create(transferPerson);
        await DbContext.SaveChangesAsync();

        foreach (var dietId in entity.DietPreferences!)
        {
            var newPersonDiet = new PersonDiets
            {
                PersonId = newPerson.Id,
                DietId = dietId
            };

            await DbContext.Set<PersonDiets>().AddAsync(newPersonDiet);
        }

        await DbContext.SaveChangesAsync();
        return newPerson;
    }
    public override async Task<Person> Update(PersonDto entity)
    {
        var updatePerson = await DbContext.Persons.FirstOrDefaultAsync(p => p.Id == entity.Id);
        if (updatePerson == null) return null!;

        updatePerson.FirstName = entity.FirstName;
        updatePerson.LastName = entity.LastName;
        updatePerson.Email = entity.Email;
        updatePerson.Gender = entity.Gender;
        updatePerson.DateOfBirth = entity.DateOfBirth;
        updatePerson.Street = entity.Street;
        updatePerson.HouseNumber = entity.HouseNumber;
        updatePerson.PostalCode = entity.PostalCode;
        updatePerson.City = entity.City;
        updatePerson.PhoneNumber = entity.PhoneNumber;
        await DbContext.SaveChangesAsync();

        var existingDiets = DbContext.PersonDiets.Where(pd => pd.PersonId == updatePerson.Id);
        DbContext.PersonDiets.RemoveRange(existingDiets);
        await DbContext.SaveChangesAsync();

        if (entity.DietPreferences != null && entity.DietPreferences.Any())
        {
            foreach (var dietId in entity.DietPreferences!)
            {
                var newPersonDiet = new PersonDiets
                {
                    PersonId = updatePerson.Id,
                    DietId = dietId
                };

                await DbContext.Set<PersonDiets>().AddAsync(newPersonDiet);
            }
        }
        
        await DbContext.SaveChangesAsync();
        return updatePerson;
    }
}