using SharedDTOs.Monitoring;
using SharedDTOs.DTOs;

namespace SharedDTOs.Events;

public class GameStartEvent : TracingEventBase
{
    public Guid BoardId { get; set; }
    public ICollection<BotDTO> Bots { get; set; }
}
