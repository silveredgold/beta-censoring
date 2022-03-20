namespace BetaCensor.Web.Status.Pages.Shared;
internal static class PageExtensions {
    internal static bool IsEntryPoint(this ManifestFile file, string entryPoint) {
        return file.IsEntryFile == true && string.Equals(System.IO.Path.GetFileNameWithoutExtension(file.SourceFile), entryPoint, StringComparison.CurrentCultureIgnoreCase);
    }
}
