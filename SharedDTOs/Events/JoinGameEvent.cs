using SharedDTOs.Monitoring;
using SharedDTOs.DTOs;

namespace SharedDTOs.Events;

public class JoinGameEvent : TracingEventBase
{
    public BotDTO Bot { get; set; }
}
