namespace BoardManager.Messaging;
public interface IMessagePublisher
{
    void PublishBoardStateUpdate(string boardFenState, Guid boardId);
    void PublishEndGameEvent(Guid boardId, Guid winnerId);
    void PublishRegisterBoard(Guid boardId);
    void PublishGUIBoardStateUpdate(string boardFenState, Guid boardId);
}
