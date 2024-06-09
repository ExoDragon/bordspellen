using Core.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Core.Infrastructure.ContextFactories;

public class EntityDatabaseContextFactory : IDesignTimeDbContextFactory<EntityDatabaseContext>
{
    public EntityDatabaseContext CreateDbContext(string[] args)
    {
        DbContextOptionsBuilder optionsBuilder = new DbContextOptionsBuilder<EntityDatabaseContext>();

        optionsBuilder.UseSqlServer(
            "Server=localhost,1434;user id=sa;password=R5CCqsn9JFZMRXZfNqgcTtT;Database=BoardGameEntity"
        );

        return new EntityDatabaseContext((DbContextOptions<EntityDatabaseContext>) optionsBuilder.Options);
    }
}