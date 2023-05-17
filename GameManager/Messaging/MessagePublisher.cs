using EasyNetQ;
using SharedDTOs.DTOs;
using SharedDTOs.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            var message = new GameStartEvent
            {
                BoardId = boardId,
                Bots = bots
            };

            foreach (var bot in bots)
            {
                _bus.PubSub.Publish(message, bot.Id.ToString());
            }

            _bus.PubSub.Publish(message, boardId.ToString());
        }
    }
}
