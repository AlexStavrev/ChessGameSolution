using EasyNetQ;
using SharedDTOs.Monitoring;
using SharedDTOs.DTOs;
using SharedDTOs.Events;
using System.Reflection;

namespace GameManager.Messaging
{
    internal class MessagePublisher : IMessagePublisher, IDisposable
    {
        private readonly IBus _bus;

        public MessagePublisher(IBus bus)
        {
            _bus = bus;
        }

        void IDisposable.Dispose()
        {
            GC.SuppressFinalize(this);
            _bus.Dispose();
        }

        public void PublishGameStart(Guid boardId, ICollection<BotDTO> bots)
        {
            using var activity = Monitoring.ActivitySource.StartActivity(MethodBase.GetCurrentMethod()!.Name);
            var message = new GameStartEvent
            {
                BoardId = boardId,
                Bots = bots
            };

            foreach (var bot in bots)
            {
                _bus.PublishWithTracingAsync<GameStartEvent>(message, bot.Id.ToString());
            }

            _bus.PublishWithTracingAsync(message, boardId.ToString());
        }
    }
}
