using System.ComponentModel.DataAnnotations;

namespace BordSpellenApi.Models;

public class LoginModel
{
    [Required, StringLength(255), EmailAddress]
    public string? Email { get; set; }

    [Required] public string? Password { get; set; }
}