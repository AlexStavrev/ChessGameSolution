using SharedDTOs.Events;
namespace BotAI.Messaging;

public interface IMessageSubscriber
{
    void HandeBoardStateUpdate(BoardStateUdpateEvent boardStaterUpadate);
    void HandleGameEndEvent(GameEndEvent gameEndEvent);
    void HandleGameStartEvent(GameStartEvent gameStartEvent);
}
