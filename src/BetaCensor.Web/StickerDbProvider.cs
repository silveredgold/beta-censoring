using BetaCensor.Web.Providers;
using CensorCore;
using LiteDB;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;

namespace BetaCensor.Web;

public class StickerDbProvider : IStickerProvider {
    private readonly StickerOptions? _options;
    private readonly ILogger<StickerDbProvider> _logger;
    private readonly LiteDatabase _db;
    private readonly List<string> _captions;
    private readonly StartupMode _mode;

    public StickerDbProvider(StickerOptions? options, CaptionOptions? captions, IEnumerable<IFileProvider> providers, ILogger<StickerDbProvider> logger) {
        _options = options;
        _logger = logger;
        _db = new LiteDatabase(":memory:");
        _captions = LoadCaptions(captions) ?? _defaultCaptions;
        _mode = _options?.StartupMode ?? StartupMode.Normal;
        var categoryProviders = providers.Where(p => p is CategoryProvider).Cast<CategoryProvider>().ToList();
        var provider = new CompositeFileProvider(providers);
        LoadStickers(provider, categoryProviders);
    }

    private List<string>? LoadCaptions(CaptionOptions? options) {
        if (options == null) return null;
        try {
            var list = options.Captions;
            if (options.FilePaths.Any()) {
                list.AddRange(options.FilePaths.Where(f => File.Exists(f) && Path.GetExtension(f) == ".txt").SelectMany(f => File.ReadAllLines(f)));
            }
            return list.Any(s => !string.IsNullOrWhiteSpace(s)) ? list : null;
        }
        catch {
            return null;
        }
    }

    public Task<string?> GetRandomCaption(string? category) {
        return Task.FromResult<string?>(_captions.Random());
    }

    public Task<RawImageData?> GetRandomImage(string imageType, float? ratio, List<string>? category) {
        if (imageType == CensorCore.Censoring.KnownAssetTypes.Stickers && _mode == StartupMode.Normal && (category ?? new List<string>()).Any() && ratio.HasValue) {
            var files = GetForCategories(category!);
            var target = files.Where(f => {
                var meta = f.Metadata;
                return meta != null && meta.ContainsKey("aspectRatio");
            }).Where(f => {
                var srcRatio = f.Metadata["aspectRatio"];
                if (srcRatio != null && srcRatio.IsDecimal && CloseEnough(srcRatio.AsDecimal, ratio.Value)) {
                    return true;
                }
                return false;
            }).Random();
            return Task.FromResult<RawImageData?>(new RawImageData(ReadFile(target), target.MimeType));
        }
        else {
            throw new NotImplementedException();
        }
    }

    public Dictionary<string, IEnumerable<RawImageData>> GetStickers() {
        var collection = _db.GetCollection<StickerRecord>("stickers");
        var files = _db.FileStorage.FindAll();
        var dict = files.GroupBy(f => f.Id.TrimStart('$', '/').Split('/').First()).ToDictionary(k => k.Key, v => v.Select(fs => new RawImageData(ReadFile(fs), fs.MimeType)));
        return dict;
    }

    private IEnumerable<LiteFileInfo<string>> GetForCategory(string category) {
        // return _db.FileStorage.Find($"{category}/"); // blocked by mbdavid/LiteDB#1889
        return _db.FileStorage.FindAll().Where(f => f.Id.StartsWith($"$/{category}/"));
    }

    private IEnumerable<LiteFileInfo<string>> GetForCategories(List<string> categories) {
        var allFiles = _db.FileStorage.FindAll();
        var files = allFiles.Where(f => categories.Any(cat => f.Id.StartsWith($"$/{cat}/"))).ToList();
        return files;
    }

#pragma warning disable 1998
    public async Task<IEnumerable<RawImageData>> GetImages(string imageType, List<string>? category) {
        if (imageType == CensorCore.Censoring.KnownAssetTypes.Stickers && category is not null && category.Any()) {
            var files = GetForCategories(category);
            // var files = category.SelectMany(cat => _db.FileStorage.Find($"$/{cat}").ToList()).ToList();
            return files.Where(f => f.Length > 0).Select(fs => new RawImageData(ReadFile(fs), fs.MimeType));
        }
        else {
            return Array.Empty<RawImageData>();
        }
    }
#pragma warning restore 1998

    private byte[] ReadFile(LiteFileInfo<string> fi) {
        using var readStream = fi.OpenRead();
        var ms = new MemoryStream();
        readStream.CopyTo(ms);
        return ms.ToArray();
    }

    private bool CloseEnough(decimal srcRatio, float targetRatio) {
        var stickerRatio = Convert.ToSingle(srcRatio);
        var margin = _options?.RatioMargin ?? 25F;
        var lowerBound = 1 - (margin / 100);
        var upperBound = 1 + (margin / 100);
        var diff = stickerRatio / targetRatio;
        return lowerBound <= diff && diff <= upperBound;
    }

    private static IEnumerable<string> GetCategories(IFileProvider contentProvider, IEnumerable<CategoryProvider> categoryProviders) {
        var providedCats = categoryProviders.Select(p => p.Category).ToList();
        var availableCats = contentProvider.GetDirectoryContents(string.Empty).Where(f => f.IsDirectory && f.Exists).Select(f => f.Name).ToList();
        return providedCats.Concat(availableCats).Distinct();
    }

    private void LoadStickers(IFileProvider contentProvider, IEnumerable<CategoryProvider> categoryProviders) {
        var cats = GetCategories(contentProvider, categoryProviders).ToList();
        var images = GetProviderImages(cats, contentProvider);
        // var collection = _db.GetCollection<StickerRecord>("stickers");
        var records = images.SelectMany(img => img.Value.Select(i => new StickerRecord(img.Key, i.Name)));

        foreach (var category in images) {
            foreach (var image in category.Value) {
                try {
                    using var read = image.CreateReadStream();
                    var record = new StickerRecord(category.Key, image.Name);
                    if (_mode == StartupMode.Hybrid) {
                        var dbCatFile = _db.FileStorage.Upload($"$/{category.Key}/{image.Name}-{image.LastModified.Millisecond}", image.Name, read);
                    }
                    else {
                        var img = SixLabors.ImageSharp.Image.Identify(read, out var format);
                        decimal srcRatio = img.Width / img.Height;
                        var meta = new BsonDocument();
                        meta["aspectRatio"] = srcRatio;
                        meta["width"] = img.Width;
                        meta["height"] = img.Height;
                        meta["format"] = format.DefaultMimeType;
                        meta["extension"] = format.FileExtensions.First();
                        read.Dispose();
                        using var readFile = image.CreateReadStream();
                        var dbCatFile = _db.FileStorage.Upload($"$/{category.Key}/{image.Name}-{image.LastModified.Second}", image.Name, readFile, meta);
                    }
                } catch {
                    _logger.LogWarning($"Failed to load file into stickers DB: {image.Name} ({(image.PhysicalPath ?? "unknown")}");
                    //ignored
                }
            }
        }
    }

    private Dictionary<string, List<IFileInfo>> GetProviderImages(List<string> categories, IFileProvider mergedProvider) {
        if (categories is not null && categories.Any()) {
            var results = new Dictionary<string, List<IFileInfo>>();
            foreach (var category in categories) {
                var catResults = mergedProvider.GetDirectoryContents(category).ToList();
                var nestedFiles = new List<IFileInfo>();
                foreach (var dir in catResults.Where(s => s.IsDirectory && s.Exists)) {
                    nestedFiles.AddRange(FileProviderExtensions.DirSearch(mergedProvider, dir));
                }
                var nest = nestedFiles.ToList();
                catResults.Where(f => !f.IsDirectory && f.Exists).Concat(nestedFiles).Where(fi => fi != null && fi.Exists);
                results.Add(category, catResults);
            }
            // var candidates = results.Where(fi => fi is not null && fi.Exists);
            return results;
            // return candidates.Select(c =>new RawImageData(ReadFile(c!)));
        }
        else {
            return new Dictionary<string, List<IFileInfo>>();
        }
    }

    public IEnumerable<string> GetCategories() {
        var files = _db.FileStorage.FindAll();
        var categories = files.Select(f => f.Id.TrimStart('$', '/').Split('/').First()).Distinct().ToList();
        return categories;
    }

    private static List<string> _defaultCaptions = new() {
        "beta",
        "cuck",
        "edge",
        "leak",
        "denied",
        "pathetic",
        "sissy",
        "stupid",
        "censored",
        "censored",
        "censored",
        "suck cock",
        "bitch",
        "locked",
        "stay locked",
        "strain",
        "no betas",
        "alphas only",
        "for alphas"
    };

    public class StickerRecord {
        public StickerRecord(string category, string name) {
            Id = ObjectId.NewObjectId();
            Category = category;
            Name = name;
        }
        [BsonCtor]
        public StickerRecord(ObjectId _id, string category, string name) {
            Id = _id;
            Category = category;
            Name = name;
        }
        public ObjectId Id { get; }
        public string Category { get; }
        public string Name { get; }
    }
}
