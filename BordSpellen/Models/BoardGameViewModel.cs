using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BordSpellen.Models.Generic;
using Core.Domain.Dto.Entities;
using Core.Domain.Enums;

namespace BordSpellen.Models;

public class BoardGameViewModel : BaseViewModel<BoardGameDto, BoardGameViewModel>
{
    [Display(Name = "Game name")]
    [Required(ErrorMessage = "Game name is required")]
    public string? Name { get; set; }
    
    [Display(Name = "Game description")]
    [Required(ErrorMessage = "Game description is required")]
    public string? Description { get; set; }
    
    [Display(Name = "Game genre")]
    [Required(ErrorMessage = "Game genre is required")]
    public BoardGameGenre Genre { get; set; }
    
    [Display(Name = "is game 18+")]
    [Required(ErrorMessage = "18+ check is required")]
    public bool HasAgeRestriction { get; set; }

    [Display(Name = "Game image")]
    [Required(ErrorMessage = "Game image is required")]
    public IFormFile Image { get; set; }

    [Display(Name = "Game type")]
    [Required(ErrorMessage = "Game type is required")]
    public BoardGameType Type { get; set; }
    
    [NotMapped]
    public string? Picture { get; set; }
    [NotMapped]
    public string? PictureFormat { get; set; }
    
    public override BoardGameDto ToDto(BoardGameViewModel model)
    {
        throw new NotImplementedException();
    }
}