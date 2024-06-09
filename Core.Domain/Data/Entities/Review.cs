using System.ComponentModel.DataAnnotations;
using Core.Domain.Data.Actors;

namespace Core.Domain.Data.Entities;

public class Review : Entity
{
    [Required]
    public int Rating { get; set; }

    [Required]
    public string? ReviewDescription { get; set; }
    
    //Relation
    [Required]
    public int EventId { get; set; }
    public GameEvent? GameEvent { get; set; }

    [Required]
    public int ReviewPosterId { get; set; }
    public Person? Person { get; set; }
}