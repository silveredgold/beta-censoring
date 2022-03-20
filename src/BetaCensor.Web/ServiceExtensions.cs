using BetaCensor.Web;
using BetaCensor.Web.Providers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;

namespace Microsoft.Extensions.DependencyInjection {
    public static class ServiceExtensions {
        public static IServiceCollection AddStickerService(this IServiceCollection services, IConfigurationSection stickerSection, IConfigurationSection captionSection, IWebHostEnvironment env) {
            var opts = stickerSection.Get<StickerOptions>() ?? new StickerOptions();
            var captionOpts = captionSection.Get<CaptionOptions>() ?? new CaptionOptions();
            var providers = new List<IFileProvider>();
            var defaultStorePath = Path.Join(env.ContentRootPath, "stickers");
            if (Directory.Exists(defaultStorePath)) {
                var defaultProvider = defaultStorePath.GetProvider(true);
                if (defaultProvider != null) {
                    providers.Add(defaultProvider);
                }
            }
            if (opts.LocalStores.Any()) {
                var storeProviders = opts.LocalStores.Select(p => p.GetProvider(true)).Where(p => p is not null);
                if (storeProviders != null && storeProviders.Any()) {
                    providers.AddRange(storeProviders!);
                }
            }
            if (opts.Paths.Any()) {
                foreach (var pathSet in opts.Paths) {
                    foreach (var path in pathSet.Value) {
                        var provider = path.GetProvider();
                        if (provider is not null) {
                            providers.Add(new CategoryProvider(pathSet.Key, provider));
                        }
                    }
                }
            }
            var service = new StickerProvider(opts, captionOpts, providers);
            services.AddSingleton<StickerProvider>(service);
            services.AddSingleton<CensorCore.IAssetStore, StickerProvider>(p => p.GetRequiredService<StickerProvider>());
            return services;

        }
    }
}