using System.ComponentModel.DataAnnotations;

namespace Core.Domain.Enums;

public enum BoardGameType
{
    [Display(Name="Board")] BOARD,
    [Display(Name="Cooperative")] COOPERATIVE,
    [Display(Name="Party")] PARTY,
    [Display(Name="Card")] CARD,
}