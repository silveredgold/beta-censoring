using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;

namespace BetaCensor.Web.Status {
    public static class StartupExtensions {

        public static IServiceCollection AddStatusPages<T>(this IServiceCollection services, IWebHostEnvironment env) where T : class, IServerInfoService {
            var physicalProvider = env.ContentRootFileProvider;
            var manifestEmbeddedProvider =
                new ManifestEmbeddedFileProvider(typeof(IServerInfoService).Assembly, "wwwroot");
            var compositeProvider =
                new CompositeFileProvider(physicalProvider, manifestEmbeddedProvider);
            services.AddSingleton<IFileProvider>(compositeProvider);
            services.AddRazorPages().AddApplicationPart(typeof(IVersionService).Assembly);
            return services.AddStatusServices<T>();
        }

        public static IServiceCollection AddStatusServices<T>(this IServiceCollection services) where T : class, IServerInfoService {
            services.AddSingleton<IServerInfoService, T>();
            services.AddSingleton<IVersionService, AssemblyVersionService<T>>();
            return services;
        }

        public static IApplicationBuilder UseStatusPages(this WebApplication app, IWebHostEnvironment env) {
            if (env.IsDevelopment()) {
                app.UseSpa(spa =>
                {
                    spa.Options.SourcePath = "ClientApp";
                    spa.Options.DevServerPort = 3000;
                    spa.UseProxyToSpaDevelopmentServer("http://localhost:3000");
                });
            }
            else {
                app.UseStaticFiles(new StaticFileOptions {
                    FileProvider = app.Services.GetRequiredService<IFileProvider>()
                });
            }
            app.MapRazorPages();
            return app;
        }
    }
}