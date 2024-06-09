using System.ComponentModel.DataAnnotations;

namespace Core.Domain.Enums;

public enum BoardGameGenre
{
    [Display(Name="Abstract Strategy")] ABSTRACTSTRATEGY,
    [Display(Name="Action Drafting")] ACTIONDRAFTING,
    [Display(Name="Area Control")] AREACONTROL,
    [Display(Name="Bluffing")] BLUFFING,
    [Display(Name="Card Drafting")] CARDDRAFTING,
    [Display(Name="Deck Building")] DECKBUILDING,
    [Display(Name="Dexterity")] DEXTERITY,
    [Display(Name="Educational")] EDUCATIONAL,
    [Display(Name="Engine Building")] ENGINEBUILDING,
    [Display(Name="Eurogame")] EUROGAME,
    [Display(Name="Legacy")] LEGACY,
    [Display(Name="miniature")] MINIATURE,
    [Display(Name="Roleplaying")] ROLEPLAYING,
    [Display(Name="Roll and Write")] ROLLANDWRITE,
    [Display(Name="Storytelling")] STORYTELLING,
    [Display(Name="tile Placement")] TILEPLACEMENT,
    [Display(Name="Trick Taking")] TRICKTAKING,
    [Display(Name="Wargame")] WARGAME,
    [Display(Name="Worker Placement")] WORKERPLACEMENT
}