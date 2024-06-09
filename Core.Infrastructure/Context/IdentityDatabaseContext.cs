using Core.Domain.Security;
using Duende.IdentityServer.EntityFramework.Options;
using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Core.Infrastructure.Context;

public class IdentityDatabaseContext : ApiAuthorizationDbContext<BoardGameUser> 
{
    public IdentityDatabaseContext( DbContextOptions options, 
        IOptions<OperationalStoreOptions> operationalStoreOptions) : 
        base(options, operationalStoreOptions)
    {
    }
}