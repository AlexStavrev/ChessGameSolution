using Rudzoft.ChessLib.Types;

namespace SharedDTOs.Events;

public class MoveEvent
{
    public Guid? BoardId { get; set; }
    public Guid BotId { get; set; }
    public Move Move { get; set; }
}