using BetaCensor.Core;
using BetaCensor.Web;
using BetaCensor.Web.Components;
using BetaCensor.Web.Providers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;

namespace Microsoft.Extensions.DependencyInjection {
    public static class ServiceExtensions {
        public static WebApplication UseStickerProvider(this WebApplication app) {
            var logger = app.Services.GetService<ILogger<IStickerProvider>>();
            var timer = new System.Diagnostics.Stopwatch();
            timer.Start();
            logger?.LogDebug($"Loading sticker provider...");
            app.Services.GetRequiredService<IStickerProvider>();
            logger?.LogInformation($"Loaded sticker provider in {timer.Elapsed.TotalSeconds}s!");
            return app;
        }

        public static Func<IServiceProvider, IStickerProvider> BuildStickerService(IWebHostEnvironment env, string sectionName, string captionSectionName) {

            return (p =>
            {
                var config = p.GetRequiredService<IConfiguration>();
                var stickerSection = config.GetSection(sectionName);
                var captionSection = config.GetSection(captionSectionName);
                var opts = stickerSection.Get<StickerOptions>() ?? new StickerOptions();
                var captionOpts = captionSection.Get<CaptionOptions>() ?? new CaptionOptions();
                var components = p.GetServices<IStoreComponent>();

                var providers = new List<IFileProvider>();
                foreach (var component in components.Where(c => c.AssetType.Equals("stickers", StringComparison.CurrentCultureIgnoreCase))) {
                    var provider = component.GetFileProvider(env.ContentRootPath);
                    if (provider != null) {
                        providers.Add(provider);
                    }
                }
                if (opts.LocalStores.Any()) {
                    var storeProviders = opts.LocalStores.Select(p => components.GetFileProvider(p, true)).Where(p => p is not null);
                    if (storeProviders != null && storeProviders.Any()) {
                        providers.AddRange(storeProviders!);
                    }
                }
                if (opts.Paths.Any()) {
                    foreach (var pathSet in opts.Paths) {
                        foreach (var path in pathSet.Value) {
                            var provider = components.GetFileProvider(path);
                            if (provider is not null) {
                                providers.Add(new CategoryProvider(pathSet.Key, provider));
                            }
                        }
                    }
                }
                var defaultCaptionPath = Path.Join(env.ContentRootPath, "captions.txt");
                if (File.Exists(defaultCaptionPath)) {
                    captionOpts.FilePaths.Add(defaultCaptionPath);
                }
                return opts.StartupMode == StartupMode.Fast
                    ? new StickerProvider(opts, captionOpts, providers)
                    : new StickerDbProvider(opts, captionOpts, providers, p.GetRequiredService<ILogger<StickerDbProvider>>());
            });
        }

        private static IFileProvider? GetFileProvider(this IEnumerable<IStoreComponent> components, string path, bool allowNesting = false) {
            IFileProvider? provider = null;
            var matched = components.Any(sc => sc.TryProvidePath(path, allowNesting, out provider));
            if (matched && provider != null) {
                return provider;
            }
            return null;
        }

        public static IServiceCollection AddStickerService(this IServiceCollection services, IWebHostEnvironment env, string stickerSection = "Stickers", string captionSection = "Captions") {
            var defaultComponents = new List<Type>() {typeof(ArchiveProviderComponent), typeof(DirectoryPathComponent), typeof(EmbeddedFilesComponent)};
            foreach (var defaultComponent in defaultComponents)
            {
                services.AddSingleton(typeof(IStoreComponent), defaultComponent);
            }
            services.AddSingleton<IStickerProvider>(BuildStickerService(env, stickerSection, captionSection));
            // services.AddSingleton<IAssetStore>(BuildStickerService(env, stickerSection, captionSection));
            services.AddSingleton<CensorCore.IAssetStore, IStickerProvider>(p => p.GetRequiredService<IStickerProvider>());
            
            return services;
        }
    }
}