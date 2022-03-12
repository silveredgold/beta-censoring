namespace BetaCensor.Web;

internal static class CoreExtensions {
    internal static Random Rand = new Random();

    public static T Random<T>(this IEnumerable<T> input) {
        return input.ElementAt(Rand.Next(input.Count()));
    }
}