using System.Collections;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.FileProviders.Physical;

namespace BetaCensor.Web.Providers;

public class PhysicalFlatDirectory : IDirectoryContents {
    private readonly string _physicalDir;

    public PhysicalFlatDirectory(string physicalPath) {
        _physicalDir = physicalPath;
    }
    internal IEnumerable<PhysicalFileInfo> GetFiles() {
        return Directory.GetFiles(_physicalDir, "*", SearchOption.AllDirectories).Select(f => new FileInfo(f)).Where(f => f.Exists).Select(f => new PhysicalFileInfo(f)).Where(f => f.Exists);
    }
    public bool Exists => Directory.GetFiles(_physicalDir, "*", SearchOption.AllDirectories).Any();

    public IEnumerator<IFileInfo> GetEnumerator() {
        return GetFiles().GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() {
        return GetFiles().GetEnumerator();
    }
}
