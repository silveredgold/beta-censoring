using Microsoft.Extensions.DependencyInjection;
using ZiggyCreatures.Caching.Fusion;

namespace BetaCensor.Caching;

public static class CachingExtensions
{
    public static void EnableCaching(this IServiceCollection services) {
        services.AddFusionCache(CensoringCaches.Matches)
        .TryWithRegisteredMemoryCache()
        .WithOptions(opt => {
        }).WithDefaultEntryOptions(new FusionCacheEntryOptions {
            Duration = TimeSpan.FromMinutes(15),
        });
        services.AddFusionCache(CensoringCaches.Censoring)
        .TryWithRegisteredMemoryCache()
        .WithOptions(opt => {
            
        }).WithDefaultEntryOptions(new FusionCacheEntryOptions {
            Duration = TimeSpan.FromMinutes(15),
        });
    }
}
