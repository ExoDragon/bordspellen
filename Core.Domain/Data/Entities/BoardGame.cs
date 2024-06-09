#nullable enable
using System.ComponentModel.DataAnnotations;
using Core.Domain.Data.Link;
using Core.Domain.Enums;

namespace Core.Domain.Data.Entities;

public class BoardGame : Entity
{
    [Required]
    public string? Name { get; set; }
    
    [Required]
    public string? Description { get; set; }
    
    [Required]
    public BoardGameGenre Genre { get; set; }
    
    [Required]
    public bool HasAgeRestriction { get; set; }
    
    public byte[]? Image { get; set; }
    
    [Required]
    public string? ImageFormat { get; set; }

    [Required]
    public BoardGameType Type { get; set; }
    
    //Relations
    // ReSharper disable once CollectionNeverUpdated.Global
    public List<BoardGameEvents>? BoardGameEvents { get; set; }
}