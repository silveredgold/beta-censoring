using BetaCensor.Core;
using BetaCensor.Web.Providers;
using Microsoft.Extensions.FileProviders;

namespace BetaCensor.Web.Components;

public class DirectoryPathComponent : IStoreComponent {
    public string AssetType => "stickers";

    public IFileProvider? GetFileProvider(string contentRoot) {
        var defaultStorePath = Path.Join(contentRoot, "stickers");
        if (Directory.Exists(defaultStorePath)) {
            if (!Path.IsPathFullyQualified(defaultStorePath)) {
                defaultStorePath = Path.GetFullPath(defaultStorePath);
            }
            var inputProvider = new PhysicalFileProvider(defaultStorePath, Microsoft.Extensions.FileProviders.Physical.ExclusionFilters.Sensitive);
            return new NestedFilesProvider(inputProvider);
        }
        return null;
    }

    public bool TryProvidePath(string path, bool allowNesting, out IFileProvider? provider) {
        provider = null;
        if (Directory.Exists(path)) {
            if (!Path.IsPathFullyQualified(path)) {
                path = Path.GetFullPath(path);
            }
            var inputProvider = new PhysicalFileProvider(path, Microsoft.Extensions.FileProviders.Physical.ExclusionFilters.Sensitive);
            provider = allowNesting ? new NestedFilesProvider(inputProvider) : inputProvider;
            return true;
        }
        return false;
    }
}
