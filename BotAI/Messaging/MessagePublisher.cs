using EasyNetQ;
using Rudzoft.ChessLib;
using Rudzoft.ChessLib.Types;
using SharedDTOs.DTOs;
using SharedDTOs.Events;

namespace BotAI.Messaging;

internal class MessagePublisher : IMessagePublisher, IDisposable
{
    private readonly IBus _bus;
    private readonly Random _random = new Random();

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
        Thread.Sleep(_random.Next(2000, 9000));
        var message = new JoinGameEvent
        {
            Bot = bot,
        };
        _bus.PubSub.Publish(message);
    }

    public void PublishMoveEvent(Guid? boardId, Guid botId, Move move)
    {
        Console.WriteLine($"Making a move {move}");
        Thread.Sleep(_random.Next(100, 800));
        var message = new MoveEvent
        {
            BoardId = boardId,
            BotId = botId,
            Move = move,
            
        };
        _bus.PubSub.Publish(message, boardId.ToString());
        Console.WriteLine("Move published");
    }

    public void PublishRequestBoardStateUpdate(Guid id, Guid boardId)
    {
        var message = new RequestBoardUpdateEvent
        {
            RequesteeId = id,
            BoardId = boardId,
        };
        _bus.PubSub.Publish(message, boardId.ToString());
        Console.WriteLine("Requested board state update event");
    }
}
