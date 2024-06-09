using System.ComponentModel.DataAnnotations;

namespace Core.Domain.Dto.Entities;

public class ReviewDto : DtoBase
{
    [Required]
    public int Rating { get; set; }

    [Required]
    public string? ReviewDescription { get; set; }
    
}