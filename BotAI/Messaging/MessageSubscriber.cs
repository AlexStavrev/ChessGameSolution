using BotAI.Models;
using EasyNetQ;
using SharedDTOs.Events;

namespace BotAI.Messaging;

public class MessageSubscriber : IMessageSubscriber
{
    private readonly string _connectionString;
    private bool _isInGame = false;
    private readonly Bot _bot;

    private bool _isListeningToBoard = false;

    public MessageSubscriber(string connectionString, Bot bot)
    {
        _connectionString = connectionString;
        _bot = bot;
    }

    public void Start()
    {
        Console.WriteLine("Waiting for a game...");
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

        Console.WriteLine($"{_bot.Id}: Listning to board {boardId}");
        _isListeningToBoard = true;

        lock (this)
        {
            while (_isInGame)
            {
                Monitor.Wait(this);
            }
        }

        _isListeningToBoard = false;
    }

    public void HandeBoardStateUpdate(BoardStateUdpateEvent boardStateUpadate)
    {
        Console.WriteLine("Board updated");
        _bot.OnBoardStateUpdateEvent(boardStateUpadate.BoardFenState);
    }

    public void HandleGameEndEvent(GameEndEvent gameEndEvent)
    {
        Console.WriteLine("Game end event received");
        _bot.OnGameEndEvent(gameEndEvent.WinnerId);
        _isInGame = false;
        Start();
    }

    public void HandleGameStartEvent(GameStartEvent gameStartEvent)
    {
        Console.WriteLine("Game started");
        var bot = gameStartEvent.Bots.Where(bot => bot.Id.Equals(_bot.Id)).First();
        if (bot == null)
        {
            throw new ArgumentException($"Unable to find bot with id {bot.Id}");
        }
        bot!.BoardId = gameStartEvent.BoardId;

        _isInGame = true;

        Task.Factory.StartNew(() =>
            StartBoardListener(bot!.BoardId!.Value)
        );

        while (!_isListeningToBoard)
        {
            Thread.Sleep(1000);
        }
        Thread.Sleep(5000);

        _bot.OnGameStartEvent(bot!);

        Thread.Sleep(10000);
        if(_bot.IsInnactive)
        {
            _bot.RequestBoardStateUpdate();
        }
    }
}
