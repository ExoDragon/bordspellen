using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Core.Domain.Attributes;
using Core.Domain.Data.Entities;
using Core.Domain.Data.Link;
using Core.Domain.Enums;

namespace Core.Domain.Data.Actors;

public class Person : Entity
{
    [Required, StringLength(255)]
    public string? FirstName { get; set; }
    
    [Required, StringLength(255)]
    public string? LastName { get; set; }

    [Required, StringLength(255), EmailAddress]
    public string? Email { get; set; }
    
    [Required, Display(Name="Gender")]
    public GenderEnum Gender { get; set; }
    
    [Required, DataType(DataType.Date), DatePast]
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

    //Relation
    public List<PersonDiets>? DietList { get; set; }
    // ReSharper disable once CollectionNeverUpdated.Global
    public List<Review>? ReviewsPosted { get; set; }
    // ReSharper disable once CollectionNeverUpdated.Global
    public List<PersonGameEvents>? PersonGameEvents { get; set; }
    
    //Function
    [NotMapped] private string FullName => $"{FirstName} {LastName}";
    public override string ToString() => FullName;
}