using BoardManager.Messaging;
using Rudzoft.ChessLib;
using Rudzoft.ChessLib.Factories;
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

    public void StartGame()
    {
        GameBoard.NewGame();
        _messagePublisher.PublishBoardStateUpdate(GameBoard.GetFen().ToString(), Id);
    }
}
