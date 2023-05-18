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
    public int Draws { get; set; }


    private readonly IMessagePublisher _messagePublisher;

    public Bot(IMessagePublisher messagePublisher) : this(
        Guid.NewGuid(),
        GameFactory.Create(),
        null,
        BoardSide.Undefined,
        new RandomMoveStrategy(),
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

        GameBoard.NewGame();
    }

    public void OnBoardStateUpdateEvent(string boardFenState)
    {
        GameBoard = GameFactory.Create(boardFenState);
        Move? nextMove = GetNextMove();

        BoardSide currentToMove = GameBoard.Pos.SideToMove.IsWhite ? BoardSide.White : BoardSide.Black;
        if (nextMove.HasValue && nextMove.Value.IsValidMove() && currentToMove.Equals(Side))
        {
            _messagePublisher.PublishMoveEvent(BoardId, Id, nextMove.Value);
        }
    }

    public void OnGameStartEvent(BotDTO botDto)
    {
        GameBoard = GameFactory.Create(botDto.GameBoardFen);
        BoardId = botDto.BoardId;
        Side = (BoardSide)((int)botDto.Side);

        if(Side.Equals(BoardSide.White))
        {
            _messagePublisher.PublishMoveEvent(BoardId, Id, GetNextMove()!.Value);
        }
    }

    public void OnGameEndEvent(Guid winnerGuid)
    {
        if (Id.Equals(winnerGuid))
        {
            Wins++;
            Console.WriteLine("Game ended, WIN");
        }
        else if (winnerGuid.Equals(new Guid()))
        {
            Draws++;
            Console.WriteLine("Game ended, DRAW");
        }
        else
        {
            Losses++;
            Console.WriteLine("Game ended, LOSS");
        }

        GameBoard.NewGame();
        BoardId = null;
        Side = BoardSide.Undefined;

        JoinGame();
    }

    public void JoinGame()
    {
        Console.WriteLine($"{Id}: Joining game. W:{Wins} L:{Losses} D:{Draws}; Strategy: {Strategy}");
        var botDto = new BotDTO()
        {
            Id = this.Id,
            GameBoardFen = this.GameBoard.GetFen().ToString(),
            BoardId = this.BoardId,
            Side = this.Side
        };
        _messagePublisher.PublishJoinGame(botDto);
    }

    public Move? GetNextMove()
    {
        return Strategy.GetNextMove(GameBoard);
    }
}
