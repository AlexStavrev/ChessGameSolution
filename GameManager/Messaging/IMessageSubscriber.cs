using SharedDTOs.Events;

namespace GameManager.Messaging;
public interface IMessageSubscriber
{
    void HandePlayerJoinEvent(JoinGameEvent joinGameEvent);
    void HandleBoardRegisterEvent(RegisterBoardEvent registerBoardEvent);
}
