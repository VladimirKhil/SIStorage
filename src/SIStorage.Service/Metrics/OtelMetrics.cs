using System.Diagnostics.Metrics;

namespace SIStorage.Service.Metrics;

/// <summary>
/// Holds service metrics.
/// </summary>
internal sealed class OtelMetrics
{
    public const string MeterName = "SIStorage";

    public OtelMetrics(IMeterFactory meterFactory)
    {
        var meter = new Meter(MeterName);

        // TODO: implement custom service metrics
    }
}
