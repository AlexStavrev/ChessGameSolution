using SharedDTOs.DTOs;

namespace SharedDTOs.Events;

public class GameStartEvent
{
    public Guid BoardId { get; set; }
    public ICollection<BotDTO> Bots { get; set; }
}
