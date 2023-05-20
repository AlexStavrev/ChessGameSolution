using SharedDTOs.Events;

namespace BoardManager.Messaging;
public interface IMessageSubsriber
{
    void HandeMoveEvent(MoveEvent moveEvent);
    void HandleGameStartEvent(GameStartEvent gameStartEvent);
    void HandleRequestBoardStateUpdate(RequestBoardUpdateEvent requestBoardUpdateEvent);
}
