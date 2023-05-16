using Rudzoft.ChessLib;
using Rudzoft.ChessLib.MoveGeneration;
using Rudzoft.ChessLib.Types;

namespace BotAI.Strategies;
internal class FirstInMindStrategy : IBotStrategy
{
    public Move GetNextMove(IGame gameBoard)
    {
        var moves = gameBoard.Pos.GenerateMoves();
        return moves.FirstOrDefault();
    }
}
