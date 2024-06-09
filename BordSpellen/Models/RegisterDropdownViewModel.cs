using Core.Domain.Data.Entities;

namespace BordSpellen.Models;

public class RegisterDropdownViewModel
{
    public RegisterDropdownViewModel()
    {
        Diets = new List<Diet>();
    }
    
    public List<Diet>? Diets { get; set; }
}