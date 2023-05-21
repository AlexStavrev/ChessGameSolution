using BoardManager.ApiClient;
using BoardManager.Models;
using EasyNetQ;
using Microsoft.Extensions.Logging;
using SharedDTOs.Events;
using SharedDTOs.Monitoring;
using System.Reflection;

namespace BoardManager.Messaging;

internal class MessagePublisher : IMessagePublisher, IDisposable
{
    private readonly IBus _bus;
    private readonly IApiClient _apiClient;

    public MessagePublisher(IBus bus, IApiClient apiClient)
    {
        _bus = bus;
        _apiClient = apiClient;
    }

    void IDisposable.Dispose()
    {
        GC.SuppressFinalize(this);
        _bus.Dispose();
    }

    public void PublishBoardStateUpdate(string boardFenState, Guid boardId)
    {
        var message = new BoardStateUdpateEvent
        {
            BoardFenState = boardFenState,
        };
        _bus.PubSub.Publish(message, boardId.ToString());
        Monitoring.Log.LogInformation("Published board state update event...");
    }

    public async Task PublishGUIBoardStateUpdate(string boardFenState)
    {
        var request = await _apiClient.PostBoardUpdate(boardFenState);
        Monitoring.Log.LogInformation("Published GUI update event...");
    }

    public void PublishEndGameEvent(Guid boardId, Guid winnerId)
    {
        var message = new GameEndEvent
        {
            BoardId = boardId,
            WinnerId = winnerId
        };
        _bus.PubSub.Publish(message, boardId.ToString());
        Monitoring.Log.LogInformation("Published end game event...");
    }

    public void PublishRegisterBoard(Guid boardId)
    {
        var message = new RegisterBoardEvent
        {
            BoardId = boardId,
        };
        _bus.PubSub.Publish(message);
        Monitoring.Log.LogInformation("Published register event...");
    }
}
