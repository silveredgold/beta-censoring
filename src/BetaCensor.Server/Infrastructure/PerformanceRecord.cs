using BetaCensor.Core.Messaging;
using CensorCore;
using LiteDB;

namespace BetaCensor.Server.Infrastructure;

public record PerformanceRecord {
    public PerformanceRecord(CensorImageResponse response) {
        Id = response.RequestId ?? Guid.NewGuid().ToString();
        Classes = (response.ImageResult?.Results ?? new List<Classification>()).Select(c => c.Label).ToList();
        Inference = response.ImageResult?.Session;
        Censoring = response.CensoringMetadata;
    }

    // public PerformanceRecord(string id, List<string> classes, SessionMetadata inference, CensoringSession censoring) =>
    //     (Id, Classes, Inference, Censoring) = (id, classes, inference, censoring);

    #pragma warning disable 8618
    [Obsolete("Only for serialization purposes", true)]
    public PerformanceRecord()
    {
        
    }
    #pragma warning restore 8618

    public string Id {get; set; }

    public List<string> Classes { get; set;}
    public SessionMetadata? Inference { get; set;}
    public CensoringSession? Censoring { get; set;}

}
