using BetaCensor.Web.Providers;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;

namespace BetaCensor.Web.Providers;

public class CategoryProvider : IFileProvider {
    public string Category {get;}
    // private readonly string _path;
    // private readonly PhysicalFlatDirectory _directory;
    private readonly IFileProvider _provider;

    public CategoryProvider(string category, IFileProvider provider) {
        Category = category;
        // _path = physicalPath;
        _provider = provider;
    }
    public IDirectoryContents GetDirectoryContents(string subpath) {
        if (string.Equals(Category, subpath, StringComparison.CurrentCultureIgnoreCase)) {
            return new VirtualDirectory(_provider.GetAllFiles());
        }
        return new EmptyDirectory();
    }

    public IFileInfo GetFileInfo(string subpath) {
        var remainingPath = TrimSubPath(subpath);
        return _provider.GetFileInfo(remainingPath);
    }

    public IChangeToken? Watch(string filter) => null;

    private string TrimSubPath(string subPath) {
        var remainingPath = subPath.Split('/', '\\').SkipWhile(s => !string.Equals(Category, s, StringComparison.CurrentCultureIgnoreCase)).Skip(1);
        return string.Join(System.IO.Path.PathSeparator, remainingPath);
    }
}
