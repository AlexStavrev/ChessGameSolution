using BoardManager.Models;
using EasyNetQ;
using Microsoft.Extensions.Logging;
using SharedDTOs.Events;
using SharedDTOs.Monitoring;
using System.Reflection;

namespace BoardManager.Messaging;
public class MessageSubscriber : IMessageSubsriber
{
    private readonly string _connectionString;
    private readonly ChessBoard _board;

    public MessageSubscriber(string connectionString, ChessBoard board)
    {
        _connectionString = connectionString;
        _board = board;
    }

    public void Start()
    {
        var boardId = _board.Id;

        using var bus = RabbitHutch.CreateBus(_connectionString);

        bus.SubscribeWithTracingAsync<GameStartEvent>("gameStarted", HandleGameStartEvent, boardId.ToString());
        bus.SubscribeWithTracingAsync<MoveEvent>("moveEvent", HandeMoveEvent, boardId.ToString());
        bus.SubscribeWithTracingAsync<RequestBoardUpdateEvent>("requestBoardUpdateEvent", HandleRequestBoardStateUpdate, boardId.ToString());

        Monitoring.Log.LogInformation("Message listener initialized.");

        // Block the thread so that it will not exit and stop subscribing.
        lock (this)
        {
            Monitor.Wait(this);
        }
        Monitoring.Log.LogInformation("Shutting down message listener.");
    }

    public void HandeMoveEvent(MoveEvent moveEvent)
    {
        Monitoring.Log.LogInformation("Movement event received...");
        using var activity = Monitoring.ActivitySource.StartActivity(MethodBase.GetCurrentMethod()!.Name);
        if(!moveEvent.Move.HasValue)
        {
            var faultyBot = _board.Bots.Where(guid => guid.Equals(moveEvent.BotId));
            Guid winnderGuid = _board.Bots.Except(faultyBot).First().Id;
            _board.EndGame(winnderGuid, _board.GameBoard.GameEndType, _board.GameBoard.GetFen().ToString());
        }
        _board.OnPlayerMoveEvent(moveEvent.BotId, moveEvent.Move.Value);
    }

    public void HandleGameStartEvent(GameStartEvent gameStartEvent)
    {
        Monitoring.Log.LogInformation("Game started event received.");
        using var activity = Monitoring.ActivitySource.StartActivity(MethodBase.GetCurrentMethod()!.Name);
        _board.StartGame(gameStartEvent.Bots);
    }

    public void HandleRequestBoardStateUpdate(RequestBoardUpdateEvent requestBoardUpdateEvent)
    {
        Monitoring.Log.LogInformation("Board state update requested event received.");
        using var activity = Monitoring.ActivitySource.StartActivity(MethodBase.GetCurrentMethod()!.Name);
        if (_board.Bots.Select(bot => bot.Id.Equals(requestBoardUpdateEvent.RequesteeId)).Any())
        {
            _board.UpdateBoardState();
        }
        else
        {
            Monitoring.Log.LogInformation("Requestee is not one of the players!");
        }
    }
}
