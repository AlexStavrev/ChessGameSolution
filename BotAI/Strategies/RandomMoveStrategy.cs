using Rudzoft.ChessLib;
using Rudzoft.ChessLib.MoveGeneration;
using Rudzoft.ChessLib.Types;

namespace BotAI.Strategies;
internal class RandomMoveStrategy : IBotStrategy
{
    private static readonly Random _random = new();

    public Move GetNextMove(IGame gameBoard)
    {
        var moves = gameBoard.Pos.GenerateMoves();

        return moves.ElementAt(_random.Next(0, moves.Length));
    }
}
