using EasyNetQ;
using SharedDTOs.Events;

namespace BoardManager.Messaging;

internal class MessagePublisher : IMessagePublisher, IDisposable
{
    private readonly IBus _bus;

    public MessagePublisher(IBus bus)
    {
        _bus = bus;
    }

    void IDisposable.Dispose()
    {
        GC.SuppressFinalize(this);
        _bus.Dispose();
    }

    public void PublishBoardStateUpdate(string boardFenState, Guid boardId)
    {
        var message = new BoardStateUdpateEvent
        {
            BoardFenState = boardFenState,
        };
        _bus.PubSub.Publish(message, boardId.ToString());
    }

    public void PublishEndGameEvent(Guid boardId, Guid winnerId)
    {
        var message = new GameEndEvent
        {
            BoardId = boardId,
            WinnerId = winnerId
        };
        _bus.PubSub.Publish(message, boardId.ToString());
    }

    public void PublishRegisterBoard(Guid boardId)
    {
        var message = new RegisterBoardEvent
        {
            BoardId = boardId,
        };
        _bus.PubSub.Publish(message);
    }
}
