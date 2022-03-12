using System.Collections;
using Microsoft.Extensions.FileProviders;

namespace BetaCensor.Web.Providers;

public class VirtualDirectory : IDirectoryContents {
    private readonly IEnumerable<IFileInfo> _files;

    public VirtualDirectory(IEnumerable<IFileInfo> files)
    {
        _files = files;
    }
    public bool Exists => true;

    public IEnumerator<IFileInfo> GetEnumerator() {
        return _files.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() {
        return _files.GetEnumerator();
    }
}
