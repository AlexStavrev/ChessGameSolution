using SharedDTOs.Monitoring;

namespace SharedDTOs.Events;

public class GameEndEvent : TracingEventBase
{
    public Guid BoardId { get; set; }
    public Guid WinnerId { get; set; }
}
