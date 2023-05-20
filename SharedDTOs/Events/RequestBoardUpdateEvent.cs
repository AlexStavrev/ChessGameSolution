using SharedDTOs.Monitoring;

namespace SharedDTOs.Events;
public class RequestBoardUpdateEvent : TracingEventBase
{
    public Guid RequesteeId { get; set; }
    public Guid BoardId { get; set; }
}
