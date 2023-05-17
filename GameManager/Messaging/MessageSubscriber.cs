using EasyNetQ;
using SharedDTOs.Events;

namespace GameManager.Messaging;
public class MessageSubscriber : IMessageSubscriber
{
    private readonly string _connectionString;

    public MessageSubscriber(string connectionString)
    {
        _connectionString = connectionString;
    }

    public void Start()
    {
        using var bus = RabbitHutch.CreateBus(_connectionString);

        bus.PubSub.Subscribe<JoinGameEvent>("joinGameEvent", HandePlayerJoinEvent);
        bus.PubSub.Subscribe<RegisterBoardEvent>("registerBoardEvent", HandleBoardRegisterEvent);

        // Block the thread so that it will not exit and stop subscribing.
        lock (this)
        {
            Monitor.Wait(this);
        }
    }

    public void HandePlayerJoinEvent(JoinGameEvent joinGameEvent)
    {
        throw new NotImplementedException();
    }

    public void HandleBoardRegisterEvent(RegisterBoardEvent registerBoardEvent)
    {
        throw new NotImplementedException();
    }
}
