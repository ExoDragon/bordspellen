using System.ComponentModel.DataAnnotations;

namespace Core.Domain.Enums;

public enum GenderEnum
{
    [Display(Name = "Male")] MALE,
    [Display(Name="Female")] FEMALE,
    [Display(Name="Other")] OTHER,
    [Display(Name="Won't Disclose")] X,
}