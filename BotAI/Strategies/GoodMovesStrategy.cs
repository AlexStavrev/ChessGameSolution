using Rudzoft.ChessLib;
using Rudzoft.ChessLib.MoveGeneration;
using Rudzoft.ChessLib.Types;

namespace BotAI.Strategies;
public class GoodMovesStrategy : IBotStrategy
{
    public Move? GetNextMove(IGame gameBoard)
    {
        var moves = gameBoard.Pos.GenerateMoves();
        int maxScore = moves.Select(move => move.Score).Max();
        return moves.Where(move => move.Score.Equals(maxScore)).FirstOrDefault();
    }
}
