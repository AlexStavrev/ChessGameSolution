using EasyNetQ;
using SharedDTOs.Events;

namespace BotAI.Messaging;

internal class MessageSubscriber : IMessageSubscriber
{
    private string _connectionString;
    private IBus _bus;

    public MessageSubscriber(string connectionString)
    {
        _connectionString = connectionString;
    }

    public void Start()
    {
        using (_bus = RabbitHutch.CreateBus(_connectionString))
        {
            _bus.PubSub.Subscribe<BoardStateUdpateEvent>("boardStateUpdated", HandeBoardStateUpdate);

            _bus.PubSub.Subscribe<GameEndEvent>("gameEnded", HandleGameEndEvent);

            _bus.PubSub.Subscribe<GameStartEvent>("gameStarted", HandleGameStartEvent);

            // Block the thread so that it will not exit and stop subscribing.
            lock (this)
            {
                Monitor.Wait(this);
            }
        }

    }

    public void HandeBoardStateUpdate(BoardStateUdpateEvent boardStaterUpadate)
    {
        throw new NotImplementedException();
    }

    public void HandleGameEndEvent(GameEndEvent gameEndEvent)
    {
        throw new NotImplementedException();
    }

    public void HandleGameStartEvent(GameStartEvent gameStartEvent)
    {
        throw new NotImplementedException();
    }
}
