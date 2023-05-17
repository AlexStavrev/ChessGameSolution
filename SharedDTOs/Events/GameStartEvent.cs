using SharedDTOs.DTOs;

namespace SharedDTOs.Events;

public class GameStartEvent
{
    public Guid BoardId { get; set; }
    public BotDTO Bot { get; set; }
}
