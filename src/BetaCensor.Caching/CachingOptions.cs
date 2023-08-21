namespace BetaCensor.Caching
{
    public class CachingOptions
    {
        public bool EnableMatchCaching {get;set;} = true;
        public bool EnableCensoringCaching {get;set;} = false;
        public bool ResetPerformance {get;set;} = false;
    }
}