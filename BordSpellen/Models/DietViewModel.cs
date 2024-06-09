using System.ComponentModel.DataAnnotations;
using BordSpellen.Models.Generic;
using Core.Domain.Dto.Entities;

namespace BordSpellen.Models;

public class DietViewModel : BaseViewModel<DietDto, DietViewModel>
{
    [Display(Name = "Diet name")]
    [Required(ErrorMessage = "Diet name is required")]
    public string? Name { get; set; }
    
    [Display(Name = "Diet description")]
    [Required(ErrorMessage = "Diet description is required")]
    public string? Description { get; set; }
    
    public override DietDto ToDto(DietViewModel model)
    {
        return new()
        {
            Name = model.Name!,
            Description = model.Description!
        };
    }
}