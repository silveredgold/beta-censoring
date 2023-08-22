using BetaCensor.Core.Messaging;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ZiggyCreatures.Caching.Fusion;

namespace BetaCensor.Caching;


public class CachingBuilder
{
    private readonly IServiceCollection _services;

    public CachingBuilder(IServiceCollection services)
    {
        _services = services;
        EnableCache();
    }

    private void EnableCache()
    {
        _services.AddFusionCache(CensoringCaches.Matches)
        .TryWithRegisteredMemoryCache()
        .WithOptions(opt =>
        {
        }).WithDefaultEntryOptions(new FusionCacheEntryOptions
        {
            Duration = TimeSpan.FromMinutes(15),
        });
        _services.AddFusionCache(CensoringCaches.Censoring)
        .TryWithRegisteredMemoryCache()
        .WithOptions(opt =>
        {

        }).WithDefaultEntryOptions(new FusionCacheEntryOptions
        {
            Duration = TimeSpan.FromMinutes(15),
        });
    }

    public CachingBuilder AddHandler()
    {
        _services.AddTransient<IRequestHandler<CensorImageRequest, CensorImageResponse>, CachedCensorImageRequestHandler>();
        return this;
    }

    public CachingBuilder AddOptions(string sectionName = "Caching")
    {
        _services.AddSingleton<CachingOptions>(p => CachingExtensions.BuildCachingOptions(p, sectionName));
        return this;
    }

}

public static class CachingExtensions
{
    public static IServiceCollection EnableCaching(this IServiceCollection services, Action<CachingBuilder>? configure = null)
    {
        var builder = new CachingBuilder(services);
        configure?.Invoke(builder);
        return services;
    }

    public static CachingOptions BuildCachingOptions(this IServiceProvider provider, string sectionName)
    {
        var config = provider.GetRequiredService<IConfiguration>();
        var section = config.GetSection(sectionName);
        var defaults = new CachingOptions();
        if (section != null && section.Get<CachingOptions>() is var cacheOpts && cacheOpts is not null)
        {
            defaults.EnableCensoringCaching = cacheOpts.EnableCensoringCaching;
            defaults.EnableMatchCaching = cacheOpts.EnableMatchCaching;
        }
        return defaults;
    }
}
