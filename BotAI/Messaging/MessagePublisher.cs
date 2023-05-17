using EasyNetQ;
using Rudzoft.ChessLib.Types;
using SharedDTOs.DTOs;
using SharedDTOs.Events;

namespace BotAI.Messaging;

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

    public void PublishJoinGame(BotDTO bot)
    {
        var message = new JoinGameEvent
        {
            Bot = bot,
        };
        _bus.PubSub.Publish(message);
    }

    public void PublishMoveEvent(Guid? boardId, Guid botId, Move move)
    {
        var message = new MoveEvent
        {
            BoardId = boardId,
            BotId = botId,
            Move = move,
            
        };
        _bus.PubSub.Publish(message, boardId.ToString());
    }
}
