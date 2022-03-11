using System.Text.Json.Serialization;

namespace BetaCensor.Server
{
    public class CancelRequest
    {
        public List<string>? Requests {get;set;}
        [JsonPropertyName("srcId")]
        public string? SourceId {get;set;}
    }
}