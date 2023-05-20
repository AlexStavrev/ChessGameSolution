using GameManager.Messaging;
using Rudzoft.ChessLib.Types;
using Serilog;
using SharedDTOs.DTOs;
using SharedDTOs.Monitoring;
using System.Collections.Concurrent;
using System.Numerics;

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
    }

    public void OnPlayerJoinEvent(BotDTO player)
    {
        lock (_lockObject)
        {
            if (!AvailablePlayers.Contains(player))
            {
                Console.WriteLine($"Player joined {player.Id}");
                Monitoring.Log.Information("Player joined queue: {Guid}", player.Id);
                AvailablePlayers.Push(player);
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
                Console.WriteLine($"Board joined {boardId}");
                Monitoring.Log.Information("Board joined queue: {Guid}", boardId);
                AvailableBoards.Push(boardId);
            }
        }
        TryToMakeGame();
    }

    public void TryToMakeGame()
    {
        lock (_lockObject)
        {
            Console.WriteLine("Trying to make a game...");
            Console.WriteLine($"Boards: {AvailableBoards.Count}");
            Console.WriteLine($"Players: {AvailablePlayers.Count}");
            Monitoring.Log.Information("Current players: {PlayerCount}; Current boards: {BoardCount}", AvailablePlayers.Count, AvailableBoards.Count);
            if (AvailablePlayers.Count > 1 && AvailableBoards.Any())
            {
                Console.WriteLine("Enough boards and players found");
                Monitoring.Log.Information("Creating a game...");
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
                    Console.WriteLine($"Added bot {bot.Id} to game {boardId}");
                }
                bots[0].Side = BoardSide.White;
                bots[1].Side = BoardSide.Black;
                Console.WriteLine("Made a game...");
                _messagePublisher.PublishGameStart(boardId, bots);
                Monitoring.Log.Information("Created a game on board {BoardId} for players {Players}", boardId, bots);
            }
        }
    }
}
