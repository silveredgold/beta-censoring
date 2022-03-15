using System.Text.Json.Serialization;

namespace BetaCensor.Server.Pages.Shared;

public class ManifestFile
{
    [JsonPropertyName("file")]
    public string FilePath {get;set;} = string.Empty;
    [JsonPropertyName("src")]
    public string SourceFile {get;set;} = string.Empty;
    [JsonPropertyName("isEntry")]
    public bool? IsEntryFile {get;set;} = null;
    [JsonPropertyName("imports")]
    public List<string> Imports {get;set;} = new();
    [JsonPropertyName("css")]
    public List<string> Stylesheets {get;set;} = new();
    [JsonPropertyName("assets")]
    public List<string> StaticAssets {get;set;} = new();
}