using Rudzoft.ChessLib.Types;
using SharedDTOs.Monitoring;

namespace SharedDTOs.Events;

public class MoveEvent : TracingEventBase
{
    public Guid? BoardId { get; set; }
    public Guid BotId { get; set; }
    public Move? Move { get; set; }
}