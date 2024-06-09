using Core.Domain.Data.Entities;

namespace Core.Domain.Data.Link;

public class BoardGameEvents : Entity
{
    public int BoardGameId { get; set; }
    public BoardGame? BoardGame { get; set; }
    
    public int GameEventId { get; set; }
    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    public GameEvent? GameEvent { get; set; }
}