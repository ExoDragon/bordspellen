using Core.Domain.Data.Entities;

namespace BordSpellen.Models.ViewModels;

public class GameEventDropdownViewModel
{
    public GameEventDropdownViewModel()
    {
        BoardGameEvents = new List<BoardGame>();
        AvailableFoodTypes = new List<Diet>();
    }
    
    public List<BoardGame>? BoardGameEvents { get; set; }
    public List<Diet>? AvailableFoodTypes { get; set; }
    
}