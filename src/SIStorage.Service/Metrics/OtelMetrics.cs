using System.Diagnostics.Metrics;

namespace SIStorage.Service.Metrics;

/// <summary>
/// Holds service metrics.
/// </summary>
internal sealed class OtelMetrics
{
    public string MeterName { get; }

    public OtelMetrics(string meterName = "SIStorage")
    {
        var meter = new Meter(meterName);
        MeterName = meterName;

        // TODO: implement custom service metrics
    }
}
