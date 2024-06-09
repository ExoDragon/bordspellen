#nullable enable
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Core.Domain.Attributes;
using Core.Domain.Data.Actors;
using Core.Domain.Data.Link;

namespace Core.Domain.Data.Entities;

public class GameEvent : Entity
{
    [Required, StringLength(255)]
    public string? Name { get; set; }
    
    [Required]
    public string? Description { get; set; }
    
    [Required, StringLength(255)]
    public string? Street { get; set; }
    
    [Required, StringLength(255)]
    public string? HouseNumber { get; set; }
    
    [Required, StringLength(255)]
    public string? PostalCode { get; set; }
    
    [Required, StringLength(255)]
    public string? City { get; set; }
    
    [Required]
    public bool IsAdultEvent { get; set; }

    [Required]
    [DataType(DataType.DateTime)]
    [DateFuture(ErrorMessage = "Date cannot be in the past")]
    public DateTime? EventDate { get; set; }
    
    [Required]
    public int MaxAmountOfPlayers { get; set; }

    //Relation
    public int OrganiserId { get; set; }
    [ForeignKey("OrganiserId")]
    public Person? Organiser { get; set; }

    public List<Review>? ReviewsRecieved { get; set; }
    public List<BoardGameEvents>? BoardGameEvents { get; set; }
    public List<GameEventDiets>? AvailableFoodTypes { get; set; }
    public List<PersonGameEvents>? GamerGameEvents { get; set; }
    
    //Function
    [NotMapped] private string Address => $"{Street} {HouseNumber} {PostalCode}, {City}";
    public override string ToString() => Address;
}