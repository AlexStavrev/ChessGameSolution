namespace SharedDTOs.Monitoring;

public class TracingEventBase
{
    public Dictionary<string, string> Headers { get; set; } = new();
}
