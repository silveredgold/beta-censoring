using BetaCensor.Web.Performance;
using LiteDB;

namespace BetaCensor.Server;

public class PerformanceDataService : IPerformanceDataService {
    private bool disposedValue;
    private LiteDatabase? _db;

    private LiteDatabase Db { get {
        _db ??= new LiteDatabase(GetPath());
        return _db;
    }}
    private readonly ILogger<PerformanceDataService> _logger;

    private static string GetPath() {
        return "performance.db";
    }
    private static string PerformanceCollection => "requests";
    public PerformanceDataService(ILogger<PerformanceDataService> logger) {
        logger.LogTrace("Creating performance data service!");
        // _db = new LiteDatabase(GetPath());
        _logger = logger;
    }

    public static bool TryReset() {
        try {
            if (File.Exists(GetPath())) {
                File.Delete(GetPath());
                return true;
            }
        }
        catch {
            //ignored
        }
        return false;
    }

    public ILiteCollection<PerformanceRecord> GetRecordCollection() {
        return Db.GetCollection<PerformanceRecord>(PerformanceCollection);
    }

    public Task<IEnumerable<PerformanceRecord>> GetAllRecords() {
        return Task.FromResult(GetRecordCollection().FindAll());
    }

    public Task AddRecord(PerformanceRecord record) {
        var requests = GetRecordCollection();
        requests.Insert(record);
        return Task.CompletedTask;
    }

    protected virtual void Dispose(bool disposing) {
        if (!disposedValue) {
            if (disposing) {
                _logger.LogTrace("Disposing performance DB connection!");
                _db?.Dispose();
                // TODO: dispose managed state (managed objects)
            }

            // TODO: free unmanaged resources (unmanaged objects) and override finalizer
            // TODO: set large fields to null
            disposedValue = true;
        }
    }

    public void Dispose() {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
