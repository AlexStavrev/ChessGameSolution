using SharedDTOs.DTOs;

namespace GameManager.Messaging;

public interface IMessagePublisher
{
    void PublishGameStart(Guid boardId, ICollection<BotDTO> bots);
}
