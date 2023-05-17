using BoardManager.Messaging;
using Rudzoft.ChessLib;
using Rudzoft.ChessLib.Factories;
using Rudzoft.ChessLib.Types;
using SharedDTOs.DTOs;

namespace BoardManager.Models;

public class ChessBoard
{
    public Guid Id { get; set; }
    public IGame GameBoard { get; set; }
    public ICollection<BotDTO> Bots { get; set; }

    private readonly IMessagePublisher _messagePublisher;

    public ChessBoard(IMessagePublisher messagePublisher) : this(
        new Guid(),
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
        _messagePublisher.PublishEndGameEvent(Id, winnerId);
    }

    public void UpdateBoardState()
    {
        _messagePublisher.PublishBoardStateUpdate(GameBoard.GetFen().ToString(), Id);
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

        UpdateBoardState();
        if (position.IsMate)
        {
            EndGame(winnerId: botId);
        }
    }
}
