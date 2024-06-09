using System.ComponentModel.DataAnnotations;
using Core.Domain.Attributes;
using Core.Domain.Enums;

namespace Core.Domain.Dto.Actors;

public class PersonDto : DtoBase
{
    [Required, StringLength(255)]
    public string? FirstName { get; set; }
    
    [Required, StringLength(255)]
    public string? LastName { get; set; }

    [Required, StringLength(255), EmailAddress]
    public string? Email { get; set; }
    
    [Required, Display(Name="Gender")]
    public GenderEnum Gender { get; set; }
    
    [Required, DataType(DataType.Date)]
    [DatePast(ErrorMessage = "Date cannot exceed today's date")]
    public DateTime? DateOfBirth { get; set; }
    
    [Required, StringLength(255)]
    public string? Street { get; set; }
    
    [Required, StringLength(255)]
    public string? HouseNumber { get; set; }
    
    [Required, StringLength(255)]
    public string? PostalCode { get; set; }
    
    [Required, StringLength(255)]
    public string? City { get; set; }
    
    [Required, StringLength(255), Phone]
    public string? PhoneNumber { get; set; }
    
    public List<int>? DietPreferences { get; set; }
}