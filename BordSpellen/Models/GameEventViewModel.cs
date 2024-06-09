using System.ComponentModel.DataAnnotations;
using BordSpellen.Models.Generic;
using Core.Domain.Attributes;
using Core.Domain.Data.Actors;
using Core.Domain.Dto.Entities;

namespace BordSpellen.Models;

public class GameEventViewModel : BaseViewModel<GameEventDto, GameEventViewModel>
{
    [Display(Name = "Event name")]
    [Required(ErrorMessage = "Event name is required")]
    public string? Name { get; set; }
    
    [Display(Name = "Event description")]
    [Required(ErrorMessage = "Event description is required")]
    public string? Description { get; set; }
    
    [Display(Name = "Street")]
    [Required(ErrorMessage = "Street is required")]
    public string? Street { get; set; }
    
    [Display(Name = "House number")]
    [Required(ErrorMessage = "House number is required")]
    public string? HouseNumber { get; set; }

    [Display(Name = "Postal code")]
    [Required(ErrorMessage = "Postal code is required")]
    public string? PostalCode { get; set; }
    
    [Display(Name = "City")]
    [Required(ErrorMessage = "City is required")]
    public string? City { get; set; }
    
    [Display(Name = "18 plus event")]
    [Required(ErrorMessage = "18 plus event is required")]
    public bool IsAdultEvent { get; set; }
    
    [Display(Name = "Date & time of event")]
    [Required(ErrorMessage = "Date & time of event is required")]
    [DateFuture(ErrorMessage = "Date & time cannot be in the past")]
    [DataType(DataType.DateTime)]
    public DateTime? EventDate { get; set; }

    [Display(Name = "Max players")]
    [Required(ErrorMessage = "Max players is required")]
    [Range(1, 100, ErrorMessage = "Out of range (1 to 100)")]
    public int MaxAmountOfPlayers { get; set; }

    public Person? Organiser { get; set; }

    public List<int>? ReviewIds { get; set; }
    
    [Display(Name = "Board games")]
    [Required(ErrorMessage = "Board games is required")]
    public List<int>? BoardGameIds { get; set; }
    
    [Display(Name = "Food types")]
    public List<int>? DietIds { get; set; }
    
    public List<int>? GamerIds { get; set; }

    public override GameEventDto ToDto(GameEventViewModel model)
    {
        return new ()
        {
            Id = model.Id,
            Name = model.Name,
            Description = model.Description,
            Street = model.Street,
            HouseNumber = model.HouseNumber,
            PostalCode = model.PostalCode,
            City = model.City,
            IsAdultEvent = IsAdultEvent,
            EventDate = model.EventDate,
            MaxAmountOfPlayers = model.MaxAmountOfPlayers,
            Organiser = model.Organiser,
            ReviewIds = model.ReviewIds!,
            BoardGameIds = model.BoardGameIds!,
            DietIds = model.DietIds!,
            GamerIds = model.GamerIds!
        };
    }
}