using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;

namespace BetaCensor.Web.Providers {
    public class NestedFilesProvider : IFileProvider {
        private readonly IFileProvider _provider;
        private readonly Func<string, IFileInfo, string> _join;

        public NestedFilesProvider(IFileProvider provider, Func<string, IFileInfo, string>? customJoiner = null) {
            _provider = provider;
            customJoiner ??= (subpath, dir) => Path.Join(subpath, dir.Name);
            _join = customJoiner;
        }

        public IDirectoryContents GetDirectoryContents(string subpath) {
            if (!string.IsNullOrWhiteSpace(subpath)) {
                var results = _provider.GetDirectoryContents(subpath);
                var files = results.Where(r => !r.IsDirectory).ToList();
                var dirs = results.Where(r => r.IsDirectory);
                foreach (var dir in dirs) {
                    files.AddRange(_provider.DirSearch(_join(subpath, dir)));
                }
                return new VirtualDirectory(files);
            }
            else {
                return _provider.GetDirectoryContents(subpath);
            }
        }

        public IFileInfo GetFileInfo(string subpath) {
            return _provider.GetFileInfo(subpath);
        }

        public IChangeToken Watch(string filter) {
            return _provider.Watch(filter);
        }
    }
}