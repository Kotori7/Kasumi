using System.Collections.Generic;

namespace Kasumi.Telemetry;

public struct MetricPayload
{
    public string Name;

    public string Type;

    public object Value;

    public long Timestamp;

    public Dictionary<string, string> Attributes;

}