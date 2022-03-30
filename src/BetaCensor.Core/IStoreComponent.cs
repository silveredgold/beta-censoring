using Microsoft.Extensions.FileProviders;

namespace BetaCensor.Core; 
public interface IStoreComponent {
    string AssetType {get;}
    IFileProvider? GetFileProvider(string contentRoot);

    bool TryProvidePath(string path, bool allowNesting, out IFileProvider? provider);
}
