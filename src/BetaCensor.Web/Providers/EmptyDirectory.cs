using System.Collections;
using Microsoft.Extensions.FileProviders;

namespace BetaCensor.Web.Providers;

public class EmptyDirectory : IDirectoryContents {
    public bool Exists => false;

    public IEnumerator<IFileInfo> GetEnumerator() {
        return (IEnumerator<IFileInfo>)Array.Empty<IFileInfo>().GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() {
        return Array.Empty<IFileInfo>().GetEnumerator();
    }
}
