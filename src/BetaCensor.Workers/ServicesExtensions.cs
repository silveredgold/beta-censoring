using BetaCensor.Core.Messaging;
using CensorCore;
using CensorCore.Censoring;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BetaCensor.Workers {
    public class WorkerConfiguration {
        public int WorkerCount { get; set; }

    }
    public static class ServicesExtensions {
        public static IServiceCollection AddWorkers<TWorker>(this IServiceCollection services, int workerCount) where TWorker : class, IHostedService {
            for (int i = 0; i < workerCount; i++) {
                services.AddSingleton<IHostedService, TWorker>();
            }
            return services;
        }

        public static IServiceCollection AddWorkers<TWorker>(this IServiceCollection services, IConfigurationSection section, string key = "WorkerCount") where TWorker : class, IHostedService {
            var value = section.GetSection(key).Get<int>();
            if (value == default(int)) value = 2;
            Console.WriteLine($"Registering {value} '{typeof(TWorker).Name}' workers");
            return services.AddWorkers<TWorker>(value);
        }

        public static IServiceCollection AddCensoringWorkers(this IServiceCollection services, int workerCount = 1) {
            return services.AddWorkers<CensoringWorkerService>(workerCount);
        }

        public static IServiceCollection AddQueues<TRequest, TResponse>(this IServiceCollection services) where TRequest : class where TResponse : class {
            services.AddSingleton<IAsyncBackgroundQueue<TRequest>, BackgroundQueue<TRequest>>();
            services.AddSingleton<IAsyncBackgroundQueue<TResponse>, BackgroundQueue<TResponse>>();
            return services;
        }

        public static IServiceCollection AddCensoring(this IServiceCollection services, byte[] model) {

            services.AddSingleton<IImageHandler>(p => new ImageSharpHandler());
            services.AddSingleton<AIService>(p =>
            {
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

        public static IServiceCollection AddDefaultManagedRequestQueue(this IServiceCollection services, params (Func<CensorImageRequest, string?> Matcher, Predicate<string?> Filter)[] filters) {
            return services.AddSingleton<QueueValidator<CensorImageRequest>>(p => {
                var manager = new QueueValidator<CensorImageRequest>(req => req.RequestId, p.GetRequiredService<ILogger<QueueValidator<CensorImageRequest>>>());
                foreach (var validator in filters)
                {
                    manager.AddFilter(validator.Matcher, validator.Filter);
                }
                return manager;
            });
        }
    }
}