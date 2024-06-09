using System.ComponentModel.DataAnnotations;
using BordSpellen.Models.Generic;
using Core.Domain.Dto.Entities;

namespace BordSpellen.Models;

public class ReviewViewModel : BaseViewModel<ReviewDto, ReviewViewModel>
{
    
    [Display(Name = "Event Rating")]
    [Required(ErrorMessage = "a Rating is required")]
    [Range(1,5, ErrorMessage = "Rating can not exceed 5")]
    public int Rating { get; set; }

    [Display(Name = "Description")]
    [Required(ErrorMessage = "a Description is required")]
    public string? ReviewDescription { get; set; }

    public override ReviewDto ToDto(ReviewViewModel model)
    {
        return new()
        {
            Id = model.Id,
            Rating = model.Rating,
            ReviewDescription = model.ReviewDescription
        };
    }
}