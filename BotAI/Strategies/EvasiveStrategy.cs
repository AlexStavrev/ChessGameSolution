using Rudzoft.ChessLib;
using Rudzoft.ChessLib.MoveGeneration;
using Rudzoft.ChessLib.Types;
using Rudzoft.ChessLib.Enums;
using SharedDTOs.Monitoring;

namespace BotAI.Strategies;
public class EvasiveStrategy : IBotStrategy
{
    public Move? GetNextMove(IGame gameBoard)
    {
        var moveTypesPriority = new List<MoveGenerationType>()
        {
            MoveGenerationType.QuietChecks,
            MoveGenerationType.Evasions,
            MoveGenerationType.Captures,
            MoveGenerationType.Legal
        };

        var moves = new MoveList();
        foreach(var moveType in moveTypesPriority)
        {
            if (moves.Any())
            {
                break;
            }
            try
            {
                moves = gameBoard.Pos.GenerateMoves(moveType);
            }
            catch(IndexOutOfRangeException ex)
            {
                Monitoring.Log.LogGetNextMoveStrategyErrorWarning(moveType, ex.Message);
            }
        }

        if(!moves.Any())
        {
            moves = gameBoard.Pos.GenerateMoves();
        }

        return moves.FirstOrDefault();
    }
}
