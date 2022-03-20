namespace BetaCensor.Web;

public class StickerOptions {
    public List<string> LocalStores { get; set; } = new();
    public Dictionary<string, List<string>> Paths { get; set; } = new();
    public float RatioMargin {get;set;} = 25F;

}

public class CaptionOptions {
    public List<string> Captions {get;set;} = new();
    public List<string> FilePaths {get;set;} = new();
}
