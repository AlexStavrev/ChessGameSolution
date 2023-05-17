using BotAI.Messaging;
using BotAI.Strategies;
using Rudzoft.ChessLib;
using Rudzoft.ChessLib.Factories;
using Rudzoft.ChessLib.Types;

namespace BotAI.Models;

public class Bot
{
    public Guid Id { get; private set; }
    public IGame GameBoard { get; set; }
    public Guid? BoardId { get; set; }
    public BoardSide Side { get; set; }
    public IBotStrategy Strategy { get; set; }

    private readonly IMessagePublisher _messagePublisher;

    public Bot(IMessagePublisher messagePublisher)
    {
        Id = new Guid();
        GameBoard = GameFactory.Create();
        BoardId = null;
        Side = BoardSide.Undefined;
        Strategy = new FirstInMindStrategy();
        _messagePublisher = messagePublisher;
    }

    public Bot(Guid id, IGame gameBoard, BoardSide side, IBotStrategy strategy, IMessagePublisher messagePublisher)
    {
        Id = id;
        GameBoard = gameBoard;
        Side = side;
        Strategy = strategy;
        _messagePublisher = messagePublisher;
    }

    public void OnBoardStateUpdateEvent(string boardFenState)
    {
        GameBoard = GameFactory.Create(boardFenState);

        BoardSide currentToMove = GameBoard.Pos.SideToMove.IsWhite ? BoardSide.White : BoardSide.Black;
        if (currentToMove.Equals(Side))
        {
            _messagePublisher.PublishMoveEvent(BoardId, Id, GetNextMove());
        }
    }

    public Move GetNextMove()
    {
        return Strategy.GetNextMove(GameBoard);
    }
}
