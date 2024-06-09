namespace Core.Domain.DTO;

public class EntityDto
{
    public int? Id { get; set; } = null;
        
    public DateTime? DateCreated { get; set; } = null;
    public DateTime? DateUpdated { get; set; } = null;

    public virtual Entity GetNew(Entity? entity = null) {
            
        if (entity == null) {

            entity = new Entity();
        }
            
        if (Id.HasValue) {

            entity.Id = Id.Value;
        }

        if (DateCreated.HasValue) {

            entity.DateCreated = DateCreated.Value;
        }

        if (DateUpdated.HasValue) {

            entity.DateUpdated = DateUpdated.Value;
        }

        return entity;
    }
}