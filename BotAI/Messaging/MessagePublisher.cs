using EasyNetQ;
using Rudzoft.ChessLib;
using Rudzoft.ChessLib.Types;
using SharedDTOs.Monitoring;
using SharedDTOs.DTOs;
using SharedDTOs.Events;
using Microsoft.Extensions.Logging;
using System.Reflection;

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
        using var activity = Monitoring.ActivitySource.StartActivity(MethodBase.GetCurrentMethod()!.Name);
        Thread.Sleep(_random.Next(2000, 3000));
        var message = new JoinGameEvent
        {
            Bot = bot,
        };
        Monitoring.Log.LogInformation("Published join game event...");
        _bus.PublishWithTracingAsync(message);
    }

    public void PublishMoveEvent(Guid? boardId, Guid botId, Move move)
    {
        Thread.Sleep(_random.Next(100, 800));
        var message = new MoveEvent
        {
            BoardId = boardId,
            BotId = botId,
            Move = move,
            
        };
        Monitoring.Log.LogInformation("Published move event...");
        _bus.PublishWithTracingAsync(message, boardId.ToString());
    }

    public void PublishRequestBoardStateUpdate(Guid id, Guid boardId)
    {
        var message = new RequestBoardUpdateEvent
        {
            RequesteeId = id,
            BoardId = boardId,
        };
        Monitoring.Log.LogInformation("Published requested board state update event...");
        _bus.PublishWithTracingAsync(message, boardId.ToString());
    }
}
