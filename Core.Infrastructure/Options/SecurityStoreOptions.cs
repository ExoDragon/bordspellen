using Duende.IdentityServer.EntityFramework.Options;
using Microsoft.Extensions.Options;

namespace Core.Infrastructure.Options;

public class SecurityStoreOptions : IOptions<OperationalStoreOptions>
{
    public OperationalStoreOptions Value => new()
    {
        DeviceFlowCodes = new TableConfiguration("DeviceCodes"),
        PersistedGrants = new TableConfiguration("PersistedGrants"),
        
        EnableTokenCleanup = true,
        TokenCleanupBatchSize = 500,
        TokenCleanupInterval = 6000
    };
}