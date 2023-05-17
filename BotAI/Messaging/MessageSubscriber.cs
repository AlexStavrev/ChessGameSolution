using EasyNetQ;
using SharedDTOs.Events;

namespace BotAI.Messaging;

internal class MessageSubscriber : IMessageSubscriber
{
    private string _connectionString;
    private bool _isInGame = false;

    public MessageSubscriber(string connectionString)
    {
        _connectionString = connectionString;

    }

    public void Start()
    {
        var botId = Program.Bot.Id;

        using var bus = RabbitHutch.CreateBus(_connectionString);

        bus.PubSub.Subscribe<GameStartEvent>("gameStarted", HandleGameStartEvent, x => x.WithTopic($"{botId}"));

        // Block the thread so that it will not exit and stop subscribing.
        lock (this)
        {
            while (!_isInGame)
            {
                Monitor.Wait(this);
            }
        }
    }

    public void StartBoardListener(Guid boardId)
    {
        using var bus = RabbitHutch.CreateBus(_connectionString);

        bus.PubSub.Subscribe<BoardStateUdpateEvent>("boardStateUpdated", HandeBoardStateUpdate, x => x.WithTopic($"{boardId}"));

        bus.PubSub.Subscribe<GameEndEvent>("gameEnded", HandleGameEndEvent, x => x.WithTopic($"{boardId}"));

        lock (this)
        {
            while (_isInGame)
            {
                Monitor.Wait(this);
            }
        }
    }

    public void HandeBoardStateUpdate(BoardStateUdpateEvent boardStateUpadate)
    {
        Program.Bot.OnBoardStateUpdateEvent(boardStateUpadate.BoardFenState);
    }

    public void HandleGameEndEvent(GameEndEvent gameEndEvent)
    {
        Start();
        _isInGame = false;
    }

    public void HandleGameStartEvent(GameStartEvent gameStartEvent)
    {
        StartBoardListener(gameStartEvent.Bot.BoardId);
        _isInGame = true;
    }
}
