using CensorCore;

namespace BetaCensor.Server {
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
    }
}