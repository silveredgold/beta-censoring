using CensorCore;

namespace BetaCensor.Web;

public interface IStickerProvider : IAssetStore {
    Dictionary<string, IEnumerable<RawImageData>> GetStickers();
    IEnumerable<string> GetCategories();
}
