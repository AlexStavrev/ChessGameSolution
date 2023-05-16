using Rudzoft.ChessLib;
using Rudzoft.ChessLib.Types;

namespace BotAI.Strategies;
public interface IBotStrategy
{
    Move GetNextMove(IGame gameBoard);
}