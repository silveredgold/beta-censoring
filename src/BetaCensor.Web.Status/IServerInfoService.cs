namespace BetaCensor.Web {
    public interface IServerInfoService {
        Dictionary<string, bool>? GetServices();
        int? GetRequestCount();
        Dictionary<string, string> GetComponents();
    }
}