﻿using BoardManager.ApiClient;
using SharedDTOs.Events;
using SharedDTOs.Monitoring;
using EasyNetQ;

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
        _bus.PubSub.PublishWithTracingAsync(message, boardId.ToString());
        
    }

    public async Task PublishGUIBoardStateUpdate(string boardFenState)
    {
        var request = await _apiClient.PostBoardUpdate(boardFenState);
        Console.WriteLine($"Request result: {request.StatusCode}");
    }

    public void PublishEndGameEvent(Guid boardId, Guid winnerId)
    {
        var winner = winnerId.Equals(new Guid()) ? "Draw" : winnerId.ToString();
        Console.WriteLine($"Game ended; Winner: {winner}");
        var message = new GameEndEvent
        {
            BoardId = boardId,
            WinnerId = winnerId
        };
        _bus.PubSub.PublishWithTracingAsync(message, boardId.ToString());
    }

    public void PublishRegisterBoard(Guid boardId)
    {
        var message = new RegisterBoardEvent
        {
            BoardId = boardId,
        };
        _bus.PubSub.PublishWithTracingAsync(message);
    }
}
