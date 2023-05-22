using EasyNetQ;
using GameManager.Models;
using SharedDTOs.Monitoring;
using SharedDTOs.Events;
using Microsoft.Extensions.Logging;
using System.Reflection;

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

        bus.SubscribeWithTracingAsync<JoinGameEvent>("joinGameEvent", HandePlayerJoinEvent);
        bus.SubscribeWithTracingAsync<RegisterBoardEvent>("registerBoardEvent", HandleBoardRegisterEvent);

        Monitoring.Log.LogInformation("Message listener initialized.");
        // Block the thread so that it will not exit and stop subscribing.
        lock (this)
        {
            Monitor.Wait(this);
        }
        Monitoring.Log.LogInformation("Shutting down message listener.");
    }

    public void HandePlayerJoinEvent(JoinGameEvent joinGameEvent)
    {
        Monitoring.Log.LogInformation("Received player join event.");
        using var activity = Monitoring.ActivitySource.StartActivity(MethodBase.GetCurrentMethod()!.Name);
        _gamesManager.OnPlayerJoinEvent(joinGameEvent.Bot);
    }

    public void HandleBoardRegisterEvent(RegisterBoardEvent registerBoardEvent)
    {
        Monitoring.Log.LogInformation("Received board join event.");
        using var activity = Monitoring.ActivitySource.StartActivity(MethodBase.GetCurrentMethod()!.Name);
        _gamesManager.OnBoardRegisterEvent(registerBoardEvent.BoardId);
    }
}
