using BetaCensor.Core.Messaging;
using CensorCore;
using Microsoft.Extensions.DependencyInjection;

namespace BetaCensor.Workers
{
    public class CensorContext
    {
        public IServiceProvider? ServiceProvider {get; private set;}
        public CensorContext Build(byte[] model, Action<IServiceCollection>? configure = null) {
            var services = new ServiceCollection();
            services.AddCensoring(model);
            services.AddCensoringWorkers(1);
            services.AddQueues<CensorImageRequest, CensorImageResponse>();
            if (configure != null) {
                configure(services);
            }
            ServiceProvider = services.BuildServiceProvider();
            return this;
        }
    }
}