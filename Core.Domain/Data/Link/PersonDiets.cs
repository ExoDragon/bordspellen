using Core.Domain.Data.Actors;
using Core.Domain.Data.Entities;

namespace Core.Domain.Data.Link;

public class PersonDiets : Entity
{
    public int PersonId { get; set; }
    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    public Person? Person { get; set; }
    
    public int DietId { get; set; }
    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    public Diet? Diet { get; set; }
}