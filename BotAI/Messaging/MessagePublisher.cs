using EasyNetQ;
using Rudzoft.ChessLib.Types;
using SharedDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotAI.Messaging
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

        public void PublishJoinGame(Guid botId)
        {
            var message = new JoinGameEvent
            {
                BotId = botId,
            };
            _bus.PubSub.Publish(message);
        }

        public void PublishMoveEvent(Guid boardId, Guid botId, Move move)
        {
            var message = new MoveEvent
            {
                BoardId = boardId,
                BotId = botId,
                Move = move,
                
            };
            _bus.PubSub.Publish(message);
        }
    }
}
