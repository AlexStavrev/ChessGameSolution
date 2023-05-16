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


    public Bot()
    {
        Id = new Guid();
        GameBoard = GameFactory.Create();
        BoardId = null;
    }

    public Bot(Guid id, IGame gameBoard, BoardSide side, IBotStrategy strategy/*, IMessagePublisher messagePublisher*/)
    {
        Id = id;
        GameBoard = gameBoard;
    }

    }

}
