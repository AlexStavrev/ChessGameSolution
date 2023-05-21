using BotAI.Models;
using EasyNetQ;
using Microsoft.Extensions.Logging;
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
        var botId = _bot.Id;
        var subscriptionResult = _bus.PubSub.Subscribe<GameStartEvent>("gameStarted", HandleGameStartEvent, x => x.WithTopic($"{botId}"));

        Monitoring.Log.LogInformation("Message listener for game started initialized.");

        // Block the thread so that it will not exit and stop subscribing.
        lock (this)
        {
            while (!_isInGame)
            {
                Monitor.Wait(this);
            }
        }

        subscriptionResult.Dispose();
        Monitoring.Log.LogInformation("No longer listening for game started events...");
    }

    public void StartBoardListener(Guid boardId)
    {
        var sub1 = _bus.PubSub.Subscribe<BoardStateUdpateEvent>("boardStateUpdated", HandeBoardStateUpdate, x => x.WithTopic($"{boardId}"));

        var sub2 = _bus.PubSub.Subscribe<GameEndEvent>("gameEnded", HandleGameEndEvent, x => x.WithTopic($"{boardId}"));

        Monitoring.Log.LogInformation("Message listener for board updates and game end events initialized.");
        _isListeningToBoard = true;

        lock (this)
        {
            while (_isInGame)
            {
                Thread.Sleep(1000);
            }
        }

        sub1.Dispose();
        sub2.Dispose();

        _isListeningToBoard = false;

        Monitoring.Log.LogInformation("No longer listening for board updates and game end events.");
    }

    public void HandeBoardStateUpdate(BoardStateUdpateEvent boardStateUpadate)
    {
        Monitoring.Log.LogInformation("Board state event received...");
        _bot.OnBoardStateUpdateEvent(boardStateUpadate.BoardFenState);
    }

    public void HandleGameEndEvent(GameEndEvent gameEndEvent)
    {
        Monitoring.Log.LogInformation("Game end event received...");
        _bot.OnGameEndEvent(gameEndEvent.WinnerId);
        _isInGame = false;
        Start();
    }

    public void HandleGameStartEvent(GameStartEvent gameStartEvent)
    {
        Monitoring.Log.LogInformation("Game started event received...");
        var bot = gameStartEvent.Bots.Where(bot => bot.Id.Equals(_bot.Id)).First();
        if (bot == null)
        {
            Monitoring.Log.LogBotNotInStartGameEventWarning(_bot.Id, gameStartEvent);
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
