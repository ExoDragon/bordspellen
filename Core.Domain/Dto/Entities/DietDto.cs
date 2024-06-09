using System.ComponentModel.DataAnnotations;

namespace Core.Domain.Dto.Entities;

public class DietDto  : DtoBase
{
    [Required(ErrorMessage = "Diet name is required")]
    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    public string? Name { get; set; }
    
    [Required(ErrorMessage = "Diet description is required")]
    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    public string? Description { get; set; }
}