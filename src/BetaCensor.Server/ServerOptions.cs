namespace BetaCensor.Server
{
    public class ServerOptions
    {
        public int WorkerCount {get;set;} = 2;
        public bool EnableSignalR {get;set;} = true;
        public bool EnableRest {get;set;} = true;
        public string? SocketPath {get;set;}
        public string? ImageDimensions {get;set;} = null;
    }
}