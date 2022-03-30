using Microsoft.Extensions.DependencyInjection;
using ModulEngine;

namespace BetaCensor.Core; 
public interface IBetaCensorPlugin : IPlugin
{
    IServiceCollection ConfigureServices(IServiceCollection services);
}
