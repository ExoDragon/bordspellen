using Core.Infrastructure.Context;
using Core.Infrastructure.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Core.Infrastructure;

public class IdentityDatabaseContextFactory : IDesignTimeDbContextFactory<IdentityDatabaseContext>
{
    public IdentityDatabaseContext CreateDbContext(string[] args)
    {
        DbContextOptionsBuilder optionsBuilder = new DbContextOptionsBuilder<IdentityDatabaseContext>();

        optionsBuilder.UseSqlServer(
            "Server=localhost,1434;user id=sa;password=R5CCqsn9JFZMRXZfNqgcTtT;Database=BoardGameIdentity"
        );

        return new IdentityDatabaseContext((DbContextOptions<IdentityDatabaseContext>) optionsBuilder.Options, new SecurityStoreOptions());
    }
}