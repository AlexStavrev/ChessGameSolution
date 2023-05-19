using EasyNetQ;
using GameManager.Models;
using SharedDTOs.Monitoring;
using SharedDTOs.Events;

namespace GameManager.Messaging;
public class MessageSubscriber : IMessageSubscriber
{
    private readonly string _connectionString;
    private readonly GamesManager _gamesManager;

    public MessageSubscriber(string connectionString, GamesManager gamesManager)
    {
        _connectionString = connectionString;
        _gamesManager = gamesManager;
    }

    public void Start()
    {
        using var bus = RabbitHutch.CreateBus(_connectionString);

        bus.PubSub.SubscribeWithTracingAsync<JoinGameEvent>("joinGameEvent", HandePlayerJoinEvent);
        bus.PubSub.SubscribeWithTracingAsync<RegisterBoardEvent>("registerBoardEvent", HandleBoardRegisterEvent);

        // Block the thread so that it will not exit and stop subscribing.
        lock (this)
        {
            Monitor.Wait(this);
        }
    }

    public void HandePlayerJoinEvent(JoinGameEvent joinGameEvent)
    {
        _gamesManager.OnPlayerJoinEvent(joinGameEvent.Bot);
    }

    public void HandleBoardRegisterEvent(RegisterBoardEvent registerBoardEvent)
    {
        _gamesManager.OnBoardRegisterEvent(registerBoardEvent.BoardId);
    }
}
