using System.Reflection;

namespace BetaCensor.Web.Status; 
public interface IVersionService {
    string? GetServerVersion();
}

public class AssemblyVersionService<T> : IVersionService {
    public string? GetServerVersion() {
        var assembly = typeof(T).Assembly;
        var version = GetProductVersion(assembly);
        return version;
    }

    private static string? GetProductVersion(Assembly assembly) {
        try {
            object[] attributes = assembly
            .GetCustomAttributes(typeof(AssemblyInformationalVersionAttribute), false);
            return attributes.Length == 0 ?
                null :
                ((AssemblyInformationalVersionAttribute)attributes[0]).InformationalVersion;
        }
        catch {
            return null;
        }
    }
}
