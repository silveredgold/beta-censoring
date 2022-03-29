using BetaCensor.Web;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
// using SixLabors.ImageSharp;

namespace BetaCensor.Web.Status.Pages {
    public class ConfigurationModel : PageModel {
        private readonly IWebHostEnvironment env;

        public ConfigurationModel(IWebHostEnvironment env)
        {
            this.env = env;
        }

        
        public void OnGet() {
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

        public IActionResult OnGetAvailableStickerSummary([FromServices]IStickerProvider provider) {
            var results = provider.GetStickers();
            var dict = new Dictionary<string, int>();
            foreach (var cat in results)
            {
                dict.Add(cat.Key, cat.Value.Count());
            }
            return new JsonResult(dict);
        }

        public IActionResult OnGetAvailableStickers([FromServices]IStickerProvider provider) {
            var results = provider.GetStickers();
            var dict = results.ToDictionary(k => k.Key, v => v.Value.Select(id => new KeyValuePair<string, byte[]>(id.MimeType ?? GetMimeType(id.RawData), id.RawData)));
            return new JsonResult(dict);
        }

        private string GetMimeType(byte[] data) {
            var ident = SixLabors.ImageSharp.Image.Identify(data, out var format);
            return format.DefaultMimeType;
        }

        
    }

    public class PathValidationRequest {
        public bool IsStore {get;set;}
        public string? LocalPath {get;set;}
        public string? Category {get;set;}
    }
}
