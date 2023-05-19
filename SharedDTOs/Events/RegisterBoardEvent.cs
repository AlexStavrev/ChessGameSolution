using SharedDTOs.Monitoring;

namespace SharedDTOs.Events;
public class RegisterBoardEvent : TracingEventBase
{
    public Guid BoardId { get; set; }
}
