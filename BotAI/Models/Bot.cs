using BotAI.Messaging;
using BotAI.Strategies;
using Rudzoft.ChessLib;
using Rudzoft.ChessLib.Factories;
using Rudzoft.ChessLib.Types;
using SharedDTOs.DTOs;

namespace BotAI.Models;

public class Bot
{
    public Guid Id { get; private set; }
    public IGame GameBoard { get; set; }
    public Guid? BoardId { get; set; }
    public BoardSide Side { get; set; }
    public IBotStrategy Strategy { get; set; }
    public int Wins { get; private set; }
    public int Losses { get; private set; }


    private readonly IMessagePublisher _messagePublisher;

    public Bot(IMessagePublisher messagePublisher) : this(
        new Guid(),
        GameFactory.Create(),
        null,
        BoardSide.Undefined,
        new FirstInMindStrategy(),
        messagePublisher
    ) { }

    public Bot(Guid id, IGame gameBoard, Guid? boardId, BoardSide side, IBotStrategy strategy, IMessagePublisher messagePublisher)
    {
        Id = id;
        GameBoard = gameBoard;
        BoardId = boardId;
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

    public void OnGameStartEvent(BotDTO botDto)
    {
        GameBoard = botDto.GameBoard;
        BoardId = botDto.BoardId;
        Side = (BoardSide)((int)botDto.Side);

        if(Side.Equals(BoardSide.White))
        {
            _messagePublisher.PublishMoveEvent(BoardId, Id, GetNextMove());
        }
    }

    public void OnGameEndEvent(Guid winnerGuid)
    {
        if (Id.Equals(winnerGuid))
        {
            Wins++;
        }
        else
        {
            Losses++;
        }

        GameBoard = GameFactory.Create();
        BoardId = null;
        Side = BoardSide.Undefined;

        JoinGame();
    }

    public void JoinGame()
    {
        _messagePublisher.PublishJoinGame(Id);
    }

    public Move GetNextMove()
    {
        return Strategy.GetNextMove(GameBoard);
    }
}
