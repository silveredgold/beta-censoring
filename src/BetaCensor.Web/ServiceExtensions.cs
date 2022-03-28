using BetaCensor.Core;
using BetaCensor.Web;
using BetaCensor.Web.Components;
using BetaCensor.Web.Providers;
using CensorCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;

namespace Microsoft.Extensions.DependencyInjection {
    public static class ServiceExtensions {

        public static Func<IServiceProvider, StickerProvider> BuildStickerService(IWebHostEnvironment env, string sectionName, string captionSectionName) {

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
                var service = new StickerProvider(opts, captionOpts, providers);
                return service;
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
            services.AddSingleton<StickerProvider>(BuildStickerService(env, stickerSection, captionSection));
            // services.AddSingleton<IAssetStore>(BuildStickerService(env, stickerSection, captionSection));
            services.AddSingleton<CensorCore.IAssetStore, StickerProvider>(p => p.GetRequiredService<StickerProvider>());
            
            return services;
        }

        // public static IServiceCollection AddStickerService(this IServiceCollection services, IConfigurationSection stickerSection, IConfigurationSection captionSection, IWebHostEnvironment env) {
        //     var opts = stickerSection.Get<StickerOptions>() ?? new StickerOptions();
        //     var captionOpts = captionSection.Get<CaptionOptions>() ?? new CaptionOptions();
        //     var providers = new List<IFileProvider>();
        //     // var defaultStorePath = Path.Join(env.ContentRootPath, "stickers");
        //     var defaultCaptionPath = Path.Join(env.ContentRootPath, "captions.txt");
        //     // if (Directory.Exists(defaultStorePath)) {
        //     //     var defaultProvider = defaultStorePath.GetProvider(true);
        //     //     if (defaultProvider != null) {
        //     //         providers.Add(defaultProvider);
        //     //     }
        //     // }
        //     if (opts.LocalStores.Any()) {
        //         var storeProviders = opts.LocalStores.Select(p => p.GetProvider(true)).Where(p => p is not null);
        //         if (storeProviders != null && storeProviders.Any()) {
        //             providers.AddRange(storeProviders!);
        //         }
        //     }
        //     if (opts.Paths.Any()) {
        //         foreach (var pathSet in opts.Paths) {
        //             foreach (var path in pathSet.Value) {
        //                 var provider = path.GetProvider();
        //                 if (provider is not null) {
        //                     providers.Add(new CategoryProvider(pathSet.Key, provider));
        //                 }
        //             }
        //         }
        //     }
        //     if (File.Exists(defaultCaptionPath)) {
        //         captionOpts.FilePaths.Add(defaultCaptionPath);
        //     }
        //     var service = new StickerProvider(opts, captionOpts, providers);
        //     services.AddSingleton<StickerProvider>(service);
        //     services.AddSingleton<CensorCore.IAssetStore, StickerProvider>(p => p.GetRequiredService<StickerProvider>());
        //     return services;
        // }
    }
}