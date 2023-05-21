using GameManager.Messaging;
using Microsoft.Extensions.Logging;
using SharedDTOs.DTOs;
using SharedDTOs.Monitoring;
using System.Collections.Concurrent;

namespace GameManager.Models;
public class GamesManager
{
    private readonly ConcurrentStack<BotDTO> AvailablePlayers = new();
    private readonly ConcurrentStack<Guid> AvailableBoards = new();

    private readonly IMessagePublisher _messagePublisher;

    private readonly object _lockObject = new();

    public GamesManager(IMessagePublisher messagePublisher)
    {
        _messagePublisher = messagePublisher;
        Monitoring.Log.LogInformation("Initializing GameManager...");
    }

    public void OnPlayerJoinEvent(BotDTO player)
    {
        lock (_lockObject)
        {
            if (!AvailablePlayers.Contains(player))
            {
                Monitoring.Log.LogPlayerJoinQueueMessage(player.Id);
                AvailablePlayers.Push(player);
            }
            else
            {
                Monitoring.Log.LogPlayerAlreadyInQueueWarning(player.Id);
            }
        }
        TryToMakeGame();
    }

    public void OnBoardRegisterEvent(Guid boardId)
    {
        lock (_lockObject)
        {
            if (!AvailableBoards.Contains(boardId))
            {
                Monitoring.Log.LogBoardJoinQueueMessage(boardId);
                AvailableBoards.Push(boardId);
            }
            else
            {
                Monitoring.Log.LogBoardAlreadyInQueueWarning(boardId);
            }
        }
        TryToMakeGame();
    }

    public void TryToMakeGame()
    {
        lock (_lockObject)
        {
            Monitoring.Log.LogTryingToMakeAGameMessage(AvailablePlayers.Count, AvailableBoards.Count);
            if (AvailablePlayers.Count > 1 && AvailableBoards.Any())
            {
                Monitoring.Log.LogInformation("Enough boards and players found; Creating a game...");
                AvailableBoards.TryPop(out Guid boardId);

                List<BotDTO> bots = new List<BotDTO>();
                for (int i = 0; i < 2; i++)
                {
                    AvailablePlayers.TryPop(out BotDTO? bot);
                    if (bot is null)
                    {
                        continue;
                    }
                    bots.Add(bot);
                }
                bots[0].Side = BoardSide.White;
                bots[1].Side = BoardSide.Black;

                _messagePublisher.PublishGameStart(boardId, bots);
                Monitoring.Log.LogGameCreatedMessage(boardId, bots);
            }
        }
    }
}
