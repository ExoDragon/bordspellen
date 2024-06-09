using System.ComponentModel.DataAnnotations;

namespace BordSpellen.Models.DtoModels;

public class LoginViewModel
{
    [Display(Name = "Email address")]
    [Required(ErrorMessage = "Email address is required")]
    public string EmailAddress { get; set; }
    
    [Required(ErrorMessage = "Password is required")]
    [DataType(DataType.Password)]
    public string Password { get; set; }
}