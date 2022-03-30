using System.Text.Json;
using BetaCensor.Web.Performance;
using CensorCore;
using CensorCore.Censoring;

namespace BetaCensor.Server; 
public static class ServerConfigurationExtensions {
    internal static MatchOptions BuildMatchOptions(this IServiceProvider provider) {
        var config = provider.GetRequiredService<IConfiguration>();
        var logger = provider.GetRequiredService<ILogger<MatchOptions>>();
        logger.LogTrace("Getting match options from configuration!");
        var section = config.GetSection("MatchOptions");
        var defaults = MatchOptions.GetDefault();
        if (section.Exists() && section.Get<MatchOptions>() is var matchOpts && matchOpts is not null) {
            logger.LogDebug("Valid configuration section found, parsing for match options!");
            defaults.MinimumScore = matchOpts.MinimumScore == default(float) ? defaults.MinimumScore : matchOpts.MinimumScore;
            matchOpts.ClassScores ??= new();
            foreach (var classScore in matchOpts.ClassScores) {
                defaults.ClassScores[classScore.Key] = classScore.Value;
            }
        }
        return defaults;
    }
    internal static GlobalCensorOptions BuildCensorOptions(this IServiceProvider provider) {
        var config = provider.GetRequiredService<IConfiguration>();
        var section = config.GetSection("CensorOptions");
        var defaults = new CensorCore.Censoring.GlobalCensorOptions();
        if (section.Exists() && section.Get<GlobalCensorOptions>() is var matchOpts && matchOpts is not null) {
            defaults.AllowTransformers = matchOpts.AllowTransformers ?? true;
            defaults.RelativeCensorScale = matchOpts.RelativeCensorScale ?? defaults.RelativeCensorScale;
            defaults.PaddingScale = matchOpts.PaddingScale ?? defaults.PaddingScale;
        }
        return defaults;
    }

    internal static Func<IServiceProvider, IImageHandler> BuildImageHandler(ServerOptions serverOptions) {
        int? maxWidth = null;
        int? maxHeight = null;
        if (!string.IsNullOrWhiteSpace(serverOptions.ImageDimensions)) {
            try {
                var dims = serverOptions.ImageDimensions.Trim().Split(':', 'x').ToList();
                if (dims.Count == 1) {
                    maxHeight = maxWidth = ParseDimension(dims[0]);
                } else if (dims.Count == 2) {
                    maxWidth = ParseDimension(dims[0]);
                    maxHeight = ParseDimension(dims[1]);
                }
            } catch {
                //ignored
            }
        }
        return serverOptions.OptimizationMode switch 
        {
            OptimizationMode.None => p => new ImageSharpHandler(maxWidth, maxHeight),
            _ => p => new BodyAreaImageHandler(new ImageSharpHandler(maxWidth, maxHeight), serverOptions.OptimizationMode),
        };
    }

    private static int? ParseDimension(string s) {
        return int.TryParse(s, System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.CurrentCulture, out var dim) ? dim : null;
    }

    public static IServiceCollection AddPerformanceData(this IServiceCollection services) {
        _ = PerformanceDataService.TryReset();
        services.AddScoped<IPerformanceDataService, PerformanceDataService>();
        return services;
    }

    public static IServiceCollection AddOpenApi(this IServiceCollection services) {
        return services.AddSwaggerGen(opts => {
            opts.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo {
                Version = "v1",
                Title = "Beta Censoring REST API",
                Description = "The REST API for the reference Beta Censoring Server",
                License = new Microsoft.OpenApi.Models.OpenApiLicense {
                    Name = "GPL-3.0-or-later",
                    Url = new Uri("https://github.com/silveredgold/beta-censoring/blob/main/LICENSE")
                }
            });
        });
    }

    public static ServerOptions? GetServerOptions(this IConfiguration config) {
        var section = config.GetSection("Server");
        return section.Exists() ? section.Get<ServerOptions>() : null;
    }

    public static JsonSerializerOptions ConfigureJsonOptions(this JsonSerializerOptions? options) {
        options ??= new JsonSerializerOptions(JsonSerializerDefaults.Web);
        options.PropertyNameCaseInsensitive = true;
        options.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
        options.WriteIndented = true;
        options.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
        return options;
    }
}
