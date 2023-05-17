using GameManager.Messaging;
using SharedDTOs.DTOs;

namespace GameManager.Models;
public class GamesManager
{
    private readonly IList<BotDTO> AvailablePlayers = Array.Empty<BotDTO>();
    private readonly IList<Guid> AvailableBoards = Array.Empty<Guid>();

    private readonly IMessagePublisher _messagePublisher;

    public GamesManager(IMessagePublisher messagePublisher)
    {
        _messagePublisher = messagePublisher;
    }

    public void OnPlayerJoinEvent(BotDTO player)
    {
        _ = AvailablePlayers.Append(player);
        TryToMakeGame();
    }

    public void OnBoardRegisterEvent(Guid boardId)
    {
        _ = AvailableBoards.Append(boardId);
        TryToMakeGame();
    }

    public void TryToMakeGame()
    {
        if(AvailablePlayers.Count > 1 && AvailableBoards.Any())
        {
            Guid boardId = AvailableBoards.First();
            AvailableBoards.Remove(boardId);

            ICollection<BotDTO> bots = Array.Empty<BotDTO>();
            for(int i = 0; i < 2; i++)
            {
                BotDTO bot = AvailablePlayers.First();
                _ = bots.Append(bot);
                AvailablePlayers.Remove(bot);
            }

            _messagePublisher.PublishGameStart(boardId, bots);
        }
    }
}
