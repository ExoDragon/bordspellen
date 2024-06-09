using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Core.Domain.Security;

public class BoardGameUser : IdentityUser
{
    [Required]
    public string FullName { get; set; }
}