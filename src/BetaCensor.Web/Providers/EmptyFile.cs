using Microsoft.Extensions.FileProviders;

namespace BetaCensor.Web.Providers;

public class EmptyFile : IFileInfo {
    public bool Exists => false;

    public bool IsDirectory => false;

    public DateTimeOffset LastModified => DateTimeOffset.MinValue;

    public long Length => 0;

    public string Name => string.Empty;

    public string PhysicalPath => string.Empty;

    public Stream CreateReadStream() {
        throw new NotImplementedException();
    }
}
