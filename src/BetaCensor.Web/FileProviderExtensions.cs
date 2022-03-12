using BetaCensor.Web.Providers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;

namespace BetaCensor.Web;

public static class FileProviderExtensions {

    internal static IFileProvider? GetProvider(this string path, bool allowNesting = false) {
        if (File.Exists(path) && Path.GetExtension(path) == ".zip") {
            throw new NotImplementedException();
        }
        else if (Directory.Exists(path)) {
            if (!Path.IsPathFullyQualified(path)) {
                path = Path.GetFullPath(path);
            }
            var provider = new PhysicalFileProvider(path, Microsoft.Extensions.FileProviders.Physical.ExclusionFilters.Sensitive);
            return allowNesting ? new NestedFilesProvider(provider) : provider;
        }
        else if (File.Exists(path) && Path.GetExtension(path) == ".dll") {
            return new EmbeddedFileProvider(System.Reflection.Assembly.LoadFile(path));
        }
        return null;
    }

    internal static IEnumerable<IFileInfo> GetAllFiles(this IFileProvider provider) {
        var results = provider.GetDirectoryContents(string.Empty);
        var files = results.Where(r => !r.IsDirectory).ToList();
        var dirs = results.Where(r => r.IsDirectory);
        foreach (var dir in dirs) {
            files.AddRange(provider.DirSearch(dir));
        }
        return files;
    }
    internal static IEnumerable<IFileInfo> DirSearch(this IFileProvider provider, IFileInfo sDir) {
        return provider.DirSearch(sDir.Name);
    }

    internal static IEnumerable<IFileInfo> DirSearch(this IFileProvider provider, string path) {
        var contents = provider.GetDirectoryContents(path);

        foreach (var f in contents.Where(s => !s.IsDirectory)) {
            yield return f;
        }

        foreach (var d in contents.Where(s => s.IsDirectory)) {
            var nested = DirSearch(provider, d);
            foreach (var nestedResult in nested) {
                yield return nestedResult;
            }
        }
    }
}
