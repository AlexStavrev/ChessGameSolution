using SharedDTOs.Monitoring;

namespace SharedDTOs.Events;

public class BoardStateUdpateEvent : TracingEventBase
{
    public string BoardFenState { get; set; }
}
