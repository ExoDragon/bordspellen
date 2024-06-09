using System.ComponentModel.DataAnnotations;
using Core.Domain.Attributes;
using Core.Domain.Data.Actors;

namespace Core.Domain.Dto.Entities;

public class GameEventDto : DtoBase
{
    [Required(ErrorMessage = "Event name is required")]
    public string? Name { get; set; }
    
    [Required(ErrorMessage = "Event description is required")]
    public string? Description { get; set; }
    
    [Required(ErrorMessage = "Street is required")]
    public string? Street { get; set; }
    
    [Required(ErrorMessage = "House number is required")]
    public string? HouseNumber { get; set; }
    
    [Required(ErrorMessage = "Postal code is required")]
    public string? PostalCode { get; set; }
    
    [Display(Name = "City")]
    [Required(ErrorMessage = "City is required")]
    public string? City { get; set; }
    
    [Display(Name = "18 plus event")]
    [Required(ErrorMessage = "18 plus event is required")]
    public bool IsAdultEvent { get; set; }
    
    [Required(ErrorMessage = "Date & time of event is required")]
    [DateFuture(ErrorMessage = "Date & time cannot be in the past")]
    [DataType(DataType.DateTime)]
    public DateTime? EventDate { get; set; }

    [Required(ErrorMessage = "Max players is required")]
    [Range(1, 100, ErrorMessage = "Out of range (1 to 100)")]
    
    public int MaxAmountOfPlayers { get; set; }
    
    public Person? Organiser { get; set; }
    
    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    public List<int>? ReviewIds { get; set; }
    
    public List<int>? BoardGameIds { get; set; }
    
    public List<int>? DietIds { get; set; }
    
    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    public List<int>? GamerIds { get; set; }
}