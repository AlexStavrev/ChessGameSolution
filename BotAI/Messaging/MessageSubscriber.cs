using BotAI.Models;
using EasyNetQ;
using SharedDTOs.Events;
using SharedDTOs.Monitoring;

namespace BotAI.Messaging;

public class MessageSubscriber : IMessageSubscriber, IDisposable
{
    private readonly string _connectionString;
    private bool _isInGame = false;
    private readonly Bot _bot;

    private readonly IBus _bus;

    private bool _isListeningToBoard = false;

    public MessageSubscriber(string connectionString, Bot bot)
    {
        _connectionString = connectionString;
        _bot = bot;

        _bus = RabbitHutch.CreateBus(_connectionString);
    }

    public void Dispose()
    {
        _bus.Dispose();
    }

    public void Start()
    {
        Console.WriteLine("Waiting for a game...");
        var botId = _bot.Id;

        var subscriptionResult = _bus.PubSub.SubscribeWithTracingAsync<GameStartEvent>("gameStarted", HandleGameStartEvent, botId.ToString());

        // Block the thread so that it will not exit and stop subscribing.
        lock (this)
        {
            while (!_isInGame)
            {
                Monitor.Wait(this);
            }
        }

        subscriptionResult.Dispose();
    }

    public void StartBoardListener(Guid boardId)
    {
        var sub1 = _bus.PubSub.SubscribeWithTracingAsync<BoardStateUdpateEvent>("boardStateUpdated", HandeBoardStateUpdate, boardId.ToString());

        var sub2 = _bus.PubSub.SubscribeWithTracingAsync<GameEndEvent>("gameEnded", HandleGameEndEvent, boardId.ToString());

        Console.WriteLine($"{_bot.Id}: Listning to board {boardId}");
        _isListeningToBoard = true;

        lock (this)
        {
            while (_isInGame)
            {
                Thread.Sleep(1000);
            }
        }

        Console.WriteLine("No longer listening to board");

        sub1.Dispose();
        sub2.Dispose();

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
