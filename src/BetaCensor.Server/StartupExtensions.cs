using BetaCensor.Server.Infrastructure;
using CensorCore;
using CensorCore.Censoring;
using CensorCore.ModelLoader;

namespace BetaCensor.Server;

public static class StartupExtensions {

    public static IServiceCollection AddPerformanceData(this IServiceCollection services) {
        _ = PerformanceDataService.TryReset();
        services.AddScoped<IPerformanceDataService, PerformanceDataService>();
        return services;
    }

    public static IServiceCollection AddCensoring(this IServiceCollection services, byte[] model) {

        services.AddSingleton<IImageHandler>(p => new ImageSharpHandler());
        services.AddSingleton<ModelLoader>(p =>
        {
            return new ModelLoaderBuilder()
                .AddDefaultPaths()
                // .AddSearchPath(config)
                .SearchAssembly(System.Reflection.Assembly.GetEntryAssembly())
                .Build();
        });
        services.AddSingleton<AIService>(p =>
        {
            // var loader = p.GetRequiredService<ModelLoader>();
            // var model = await loader.GetModel();
            if (model == null) {
                throw new InvalidOperationException("Could not load model from any available source!");
            }
            return AIService.Create(model, p.GetRequiredService<IImageHandler>(), false);
        });

        services.AddSingleton<IAssetStore, EmptyAssetStore>();
        services.AddSingleton<GlobalCensorOptions>();
        services.AddSingleton<ICensorTypeProvider, BlurProvider>();
        services.AddSingleton<ICensorTypeProvider, PixelationProvider>();
        services.AddSingleton<ICensorTypeProvider, BlackBarProvider>();
        services.AddSingleton<ICensorTypeProvider, StickerProvider>();
        services.AddSingleton<ICensorTypeProvider, CaptionProvider>();
        services.AddSingleton<ICensoringProvider, ImageSharpCensoringProvider>();
        services.AddSingleton<IResultsTransformer, CensorScaleTransformer>();
        services.AddSingleton<IResultsTransformer, IntersectingMatchMerger>();
        services.AddSingleton<ICensoringMiddleware, FacialFeaturesMiddleware>();
        return services;
    }
}