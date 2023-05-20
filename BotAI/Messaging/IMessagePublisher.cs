using Rudzoft.ChessLib.Types;
using SharedDTOs.DTOs;

namespace BotAI.Messaging;

public interface IMessagePublisher
{
    void PublishMoveEvent(Guid? gameId, Guid botId, Move move);
    void PublishJoinGame(BotDTO botId);
    void PublishRequestBoardStateUpdate(Guid id, Guid boardId);
}
