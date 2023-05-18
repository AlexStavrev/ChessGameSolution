using BoardManager.Messaging;
using Rudzoft.ChessLib;
using Rudzoft.ChessLib.Factories;
using Rudzoft.ChessLib.Types;
using SharedDTOs.DTOs;
using Rudzoft.ChessLib.Enums;

namespace BoardManager.Models;

public class ChessBoard
{
    public Guid Id { get; set; }
    public IGame GameBoard { get; set; }
    public ICollection<BotDTO> Bots { get; set; }

    private readonly IMessagePublisher _messagePublisher;

    public ChessBoard(IMessagePublisher messagePublisher) : this(
        Guid.NewGuid(),
        GameFactory.Create(),
        new List<BotDTO>(),
        messagePublisher
    )
    { }

    public ChessBoard(Guid id, IGame gameBoard, ICollection<BotDTO> bots, IMessagePublisher messagePublisher) 
    { 
        Id = id; 
        GameBoard = gameBoard; 
        Bots = bots; 
        _messagePublisher = messagePublisher;
    }

    public void RegisterBoard()
    {
        _messagePublisher.PublishRegisterBoard(Id);
    }

    public void StartGame(ICollection<BotDTO> bots)
    {
        Bots = bots;
        GameBoard.NewGame();
        UpdateBoardState();
    }

    public void EndGame(Guid winnerId)
    {
        Thread.Sleep(2000);
        _messagePublisher.PublishEndGameEvent(Id, winnerId);

        RegisterBoard();
    }

    public void UpdateBoardState()
    {
        _messagePublisher.PublishBoardStateUpdate(GameBoard.GetFen().ToString(), Id);
    }

    public void UpdateGUIBoardState()
    {
        _messagePublisher.PublishGUIBoardStateUpdate(GameBoard.GetFen().ToString());
    }

    public void OnPlayerMoveEvent(Guid botId, Move move)
    {
        BoardSide currentSide = GameBoard.CurrentPlayer().IsWhite ? BoardSide.White : BoardSide.Black;
        BotDTO bot = Bots.Where(bot => bot.Id.Equals(botId)).First();

        if (!bot.Side.Equals(currentSide))
        {
            throw new ArgumentException($"Player with id '{botId}' is not in turn to move {bot.Side} != {currentSide}");
        }

        if(!move.IsValidMove())
        {
            throw new ArgumentException($"Invalid move '{move}'");
        }

        var position = GameBoard.Pos;
        position.MakeMove(move, position.State);

        UpdateGUIBoardState();
        GameBoard.UpdateDrawTypes();
        if (position.IsMate)
        {
            EndGame(winnerId: botId);
        }
        else if( GameBoard.GameEndType   == GameEndTypes.MaterialDrawn
           || GameBoard.GameEndType == GameEndTypes.FiftyMove
           || GameBoard.GameEndType == GameEndTypes.Repetition)
        {
            EndGame(winnerId: new Guid());
        }
        else
        {
            UpdateBoardState();
        }
    }
}
