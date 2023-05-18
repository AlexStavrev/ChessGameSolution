using GameManager.Messaging;
using SharedDTOs.DTOs;
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
    }

    public void OnPlayerJoinEvent(BotDTO player)
    {
        lock (_lockObject)
        {
            if (!AvailablePlayers.Contains(player))
            {
                Console.WriteLine($"Player joined {player.Id}");
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
            if (AvailablePlayers.Count > 1 && AvailableBoards.Any())
            {
                Console.WriteLine("Enough boards and players found");
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
            }
        }
    }
}
