namespace BetaCensor.Web.Performance {
    public interface IPerformanceDataService : IDisposable {
        Task<IEnumerable<PerformanceRecord>> GetAllRecords();
        Task AddRecord(PerformanceRecord record);
    }
}