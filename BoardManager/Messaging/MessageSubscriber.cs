using BoardManager.Models;
using EasyNetQ;
using SharedDTOs.Events;

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

        bus.PubSub.Subscribe<GameStartEvent>("gameStarted", HandleGameStartEvent, x => x.WithTopic($"{boardId}"));
        bus.PubSub.Subscribe<MoveEvent>("moveEvent", HandeMoveEvent, x => x.WithTopic($"{boardId}"));

        // Block the thread so that it will not exit and stop subscribing.
        lock (this)
        {
            Monitor.Wait(this);
        }
    }

    public void HandeMoveEvent(MoveEvent moveEvent)
    {
        _board.OnPlayerMoveEvent(moveEvent.BotId, moveEvent.Move);
    }

    public void HandleGameStartEvent(GameStartEvent gameStartEvent)
    {
        _board.StartGame(gameStartEvent.Bots);
    }
}
