using BotAI.Models;
using EasyNetQ;
using SharedDTOs.Events;

namespace BotAI.Messaging;

public class MessageSubscriber : IMessageSubscriber
{
    private readonly string _connectionString;
    private bool _isInGame = false;
    private readonly Bot _bot;

    public MessageSubscriber(string connectionString, Bot bot)
    {
        _connectionString = connectionString;
        _bot = bot;
    }

    public void Start()
    {
        var botId = _bot.Id;

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
        _bot.OnBoardStateUpdateEvent(boardStateUpadate.BoardFenState);
    }

    public void HandleGameEndEvent(GameEndEvent gameEndEvent)
    {
        _bot.OnGameEndEvent(gameEndEvent.WinnerId);
        _isInGame = false;
        Start();
    }

    public void HandleGameStartEvent(GameStartEvent gameStartEvent)
    {
        _bot.OnGameStartEvent(gameStartEvent.Bot);
        _isInGame = true;
        StartBoardListener(gameStartEvent.Bot.BoardId);
    }
}
