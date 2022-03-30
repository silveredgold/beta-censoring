using BetaCensor.Core;
using Microsoft.Extensions.FileProviders;

namespace BetaCensor.Web.Components;

public class EmbeddedFilesComponent : IStoreComponent {
    public string AssetType => "stickers";

    public IFileProvider? GetFileProvider(string contentRoot) {
        return null;
    }

    public bool TryProvidePath(string path, bool allowNesting, out IFileProvider? provider) {
        if (File.Exists(path) && Path.GetExtension(path) == ".dll") {
            provider = new EmbeddedFileProvider(System.Reflection.Assembly.LoadFile(path));
            return true;
        }
        provider = null;
        return false;
    }
}
