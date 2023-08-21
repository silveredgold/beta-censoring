using BetaCensor.Core.Messaging;

namespace BetaCensor.Caching;

internal static class CoreExtensions
{
    internal static string GetKey(this CensorImageRequest request)
    {
        return (string.IsNullOrWhiteSpace(request.ImageDataUrl) ? request.ImageUrl : request.ImageDataUrl) ?? request.GetHashCode().ToString();
    }

    internal static string GetHash(this string input)
    {
        // Use input string to calculate MD5 hash
        using System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create();
        byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
        byte[] hashBytes = md5.ComputeHash(inputBytes);

        return Convert.ToHexString(hashBytes);
    }
    internal static string GetHash(this IEnumerable<char> input)
    {
        // Use input string to calculate MD5 hash
        return new string(input.ToArray()).GetHash();
    }
}
