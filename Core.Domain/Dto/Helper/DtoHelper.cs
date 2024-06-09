using Core.Domain.Data.Actors;
using Core.Domain.Dto.Actors;

namespace Core.Domain.Dto.Helper;

public static class DtoHelper
{
    public static Person ReturnToDomainModel(this PersonDto model)
    {
        return new()
        {
            Id = model.Id,
            FirstName = model.FirstName,
            LastName = model.LastName,
            Email = model.Email,
            Gender = model.Gender,
            DateOfBirth = model.DateOfBirth,
            Street = model.Street,
            HouseNumber = model.HouseNumber,
            PostalCode = model.PostalCode,
            City = model.City,
            PhoneNumber = model.PhoneNumber
        };
    }
}