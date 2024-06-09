using System.ComponentModel.DataAnnotations;
using Core.Domain.Data.Link;

namespace Core.Domain.Data.Entities;

public class Diet : Entity
{
    [Required]
    public string? Name { get; set; }
    
    [Required]
    public string? Description { get; set; }
    
    //Relations
    // ReSharper disable once CollectionNeverUpdated.Global
    public List<PersonDiets>? Persons { get; set; }
    // ReSharper disable once CollectionNeverUpdated.Global
    public List<GameEventDiets>? GameEventDiets { get; set; }
}