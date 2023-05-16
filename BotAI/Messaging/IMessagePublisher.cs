using Rudzoft.ChessLib.Types;

namespace BotAI.Messaging;

public interface IMessagePublisher
{
    void PublishMoveEvent(Guid? gameId, Guid botId, Move move);
    void PublishJoinGame(Guid botId);
}
