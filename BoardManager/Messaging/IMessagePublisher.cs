namespace BoardManager.Messaging;
public interface IMessagePublisher
{
    void PublishBoardStateUpdate(string boardFenState);
    void PublishEndGameEvent(Guid boardId, Guid winnerId);
    void PublishRegisterBoard(Guid boardId);
}
