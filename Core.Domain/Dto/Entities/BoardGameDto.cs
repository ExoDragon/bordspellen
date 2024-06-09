using System.ComponentModel.DataAnnotations;
using Core.Domain.Enums;

namespace Core.Domain.Dto.Entities;

public class BoardGameDto : DtoBase
{
    [Required]
    public string? Name { get; set; }
    
    [Required]
    public string? Description { get; set; }
    
    [Required]
    public BoardGameGenre Genre { get; set; }
    
    [Required]
    public bool HasAgeRestriction { get; set; }

    [Required]
    public string? Image { get; set; }

    [Required]
    public BoardGameType Type { get; set; }
}