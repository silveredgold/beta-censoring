using CensorCore;
using CensorCore.Censoring;
using Microsoft.Extensions.Logging;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace BetaCensor.Core {
    public class ObfuscationMiddleware : ICensoringMiddleware {
        private readonly FontCollection _fontCollection;
        private readonly ILogger<ObfuscationMiddleware> _logger;

        public ObfuscationMiddleware(ILogger<ObfuscationMiddleware> logger) {
            _fontCollection = new EmbeddedFontProvider().LoadEmbeddedFonts().GetCollection();
            _logger = logger;
        }

        public Task OnAfterCensoring(Image image, IResultParser? parser) {
            if (parser?.GetOptions("_OBFUSCATION") is var opts && !string.IsNullOrWhiteSpace(opts?.CensorType) && opts.CensorType.Equals("obfuscate", StringComparison.CurrentCultureIgnoreCase)) {
                var timer = new System.Diagnostics.Stopwatch();
                timer.Start();
                var fi = image;
                var binary = fi.Clone(x =>
                {
                    x.BinaryDither(KnownDitherings.FloydSteinberg);
                });
                Console.WriteLine($"text: " + timer.Elapsed.TotalSeconds);
                fi.Mutate(x =>
                {
                    // x.Dither(KnownDitherings.Bayer2x2);
                    // x.Dither(KnownDitherings.Bayer16x16, 0.1F);
                    // Console.WriteLine("init: " + timer.Elapsed.TotalSeconds);
                    
                    // Console.WriteLine("dith: " + timer.Elapsed.TotalSeconds);
                    x.GaussianBlur(0.5F);
                    // Console.WriteLine("blur: " + timer.Elapsed.TotalSeconds);
                    x.Kodachrome();
                    // Console.WriteLine("koda: " + timer.Elapsed.TotalSeconds);
                    x.DrawImage(binary, new Point(10, 10), 0.35F);
                    x.Dither(KnownDitherings.FloydSteinberg, 0.75F);
                    // Console.WriteLine("draw: " + timer.Elapsed.TotalSeconds);
                    
                    var ctr = new System.Numerics.Vector2(fi.Width / 2, fi.Height / 2);
                    IBrush brush = Brushes.Solid(Color.White.WithAlpha(0.25F));
                    var rand = new Random().Next(-100, 100) / 100.0F;
                    var lineThick = Math.Abs(rand) * 10F;
                    x.DrawLines(brush, lineThick, new PointF(0, 0), new PointF(fi.Width, fi.Height));
                    x.DrawLines(brush, lineThick, new PointF(fi.Width, 0), new PointF(0, fi.Height));
                });
                _logger.LogInformation($"Obfuscation enabled! Image obfuscated in {timer.Elapsed.TotalSeconds}s.");
            }

            return Task.CompletedTask;
        }

        public Task<IEnumerable<Classification>?> OnBeforeCensoring(ImageResult image, IResultParser? parser, Action<int, Action<IImageProcessingContext>> addLateMutation) {
            return Task.FromResult<IEnumerable<Classification>?>(null);
        }
    }
}