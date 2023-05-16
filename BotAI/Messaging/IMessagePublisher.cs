using Rudzoft.ChessLib.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotAI.Messaging
{
    public interface IMessagePublisher
    {
        void PublishMoveEvent(Guid gameId, Guid botId, Move move);
        void PublishJoinGame(Guid botId);
    }
}
