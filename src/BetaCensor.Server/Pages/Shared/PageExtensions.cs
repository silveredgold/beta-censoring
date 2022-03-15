namespace BetaCensor.Server.Pages.Shared;
internal static class PageExtensions {
    internal static bool IsEntryPoint(this Pages.Shared.ManifestFile file, string entryPoint) {
        return file.IsEntryFile == true && string.Equals(System.IO.Path.GetFileNameWithoutExtension(file.SourceFile), entryPoint, StringComparison.CurrentCultureIgnoreCase);
    }
}
