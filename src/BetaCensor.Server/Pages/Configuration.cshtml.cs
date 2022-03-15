using BetaCensor.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
// using SixLabors.ImageSharp;

namespace MyApp.Namespace {
    public class ConfigurationModel : PageModel {
        private readonly IWebHostEnvironment env;

        public ConfigurationModel(IWebHostEnvironment env)
        {
            this.env = env;
        }

        
        public void OnGet() {
        }

        public async Task<IActionResult> OnGetAssets([FromServices] BetaCensor.Web.StickerProvider stickerProvider) {
            var stickers = await stickerProvider.GetCategories();
            return new JsonResult(new
            {
                stickerCategories = stickers
            });
        }

        public IActionResult OnGetCurrentConfiguration([FromServices] IConfiguration config) {
            var section = config.GetSection("Stickers");
            StickerOptions? opts = null;
            if (section.Exists()) {
                opts = section.Get<StickerOptions>();
            }
            return new JsonResult(new
            {
                stickerOptions = opts
            });
        }

        

        public IActionResult OnPostValidatePath(PathValidationRequest request) {
            if (!string.IsNullOrWhiteSpace(request.LocalPath)) {
                DirectoryInfo? dirInfo = null;
                try {
                    dirInfo = new DirectoryInfo(request.LocalPath);
                } catch {
                    //ignored
                }
                if (dirInfo == null || !dirInfo.Exists) {
                    return new UnprocessableEntityResult();
                }
                if (request.IsStore && dirInfo.GetDirectories().Count() == 0) {
                    return new StatusCodeResult(StatusCodes.Status412PreconditionFailed);
                }
                if (dirInfo.GetFileSystemInfos("*.*", SearchOption.AllDirectories).Any(f => f is FileInfo)) {
                    return new AcceptedResult();
                }
                return new UnprocessableEntityResult();
            }
            return BadRequest();
        }

        public async Task<IActionResult> OnGetAvailableStickerSummary([FromServices]StickerProvider provider) {
            var cats = await provider.GetCategories();
            var dict = new Dictionary<string, int>();
            foreach (var cat in cats)
            {
                var images = await provider.GetImages(CensorCore.Censoring.KnownAssetTypes.Stickers, new List<string> {cat});
                dict.Add(cat, images.Count());
            }
            return new JsonResult(dict);
        }

        public async Task<IActionResult> OnGetAvailableStickers([FromServices]StickerProvider provider) {
            var cats = await provider.GetCategories();
            var dict = new Dictionary<string, IEnumerable<KeyValuePair<string, byte[]>>>();
            foreach (var cat in cats)
            {
                var images = await provider.GetImages(CensorCore.Censoring.KnownAssetTypes.Stickers, new List<string> {cat});
                dict.Add(cat, images.Select(i => {
                    var ident = SixLabors.ImageSharp.Image.Identify(i.RawData, out var format);
                    return new KeyValuePair<string, byte[]>(format.DefaultMimeType, i.RawData);
                }));
            }
            return new JsonResult(dict);
        }

        
    }

    public class PathValidationRequest {
        public bool IsStore {get;set;}
        public string? LocalPath {get;set;}
        public string? Category {get;set;}
    }
}
