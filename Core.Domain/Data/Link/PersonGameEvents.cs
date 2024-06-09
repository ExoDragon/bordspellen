using Core.Domain.Data.Actors;
using Core.Domain.Data.Entities;

namespace Core.Domain.Data.Link;

public class PersonGameEvents : Entity
{
    public int PersonId { get; set; }
    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    public Person? Person { get; set; }
    
    public int GameEventId { get; set; }
    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    public GameEvent? GameEvent { get; set; }
}