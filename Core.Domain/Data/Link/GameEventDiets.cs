using Core.Domain.Data.Entities;

namespace Core.Domain.Data.Link;

public class GameEventDiets : Entity
{
    public int GameEventId { get; set; }
    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    public GameEvent? GameEvent { get; set; }
    
    public int DietId { get; set; }
    public Diet? Diet { get; set; }
}