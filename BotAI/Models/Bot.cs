﻿using BotAI.Messaging;
using BotAI.Strategies;
using Microsoft.Extensions.Logging;
using Rudzoft.ChessLib;
using Rudzoft.ChessLib.Factories;
using Rudzoft.ChessLib.Types;
using SharedDTOs.DTOs;
using SharedDTOs.Monitoring;
using System.Reflection;

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
    public bool IsInnactive { get; set; }
    public DateTime LastMoved { get; set; }

    private readonly Random _random = new Random();
    private readonly IMessagePublisher _messagePublisher;

    public Bot(IMessagePublisher messagePublisher) : this(
        Guid.NewGuid(),
        GameFactory.Create(),
        null,
        BoardSide.Undefined,
        new RandomMoveStrategy(),
        messagePublisher
    )
    { }

    public Bot(Guid id, IGame gameBoard, Guid? boardId, BoardSide side, IBotStrategy strategy, IMessagePublisher messagePublisher)
    {
        Id = id;
        GameBoard = gameBoard;
        BoardId = boardId;
        Side = side;
        Strategy = strategy;
        _messagePublisher = messagePublisher;
        IsInnactive = true;

        GameBoard.NewGame();
    }

    public void OnBoardStateUpdateEvent(string boardFenState)
    {
        Monitoring.Log.LogBoardStateUpdateMessage(boardFenState);
        GameBoard = GameFactory.Create(boardFenState);

        Thread.Sleep(_random.Next(100, 200));

        BoardSide currentToMove = GameBoard.Pos.SideToMove.IsWhite ? BoardSide.White : BoardSide.Black;
        MakeMove(GetNextMove()!.Value, currentToMove);
    }

    public void OnGameStartEvent(BotDTO botDto)
    {
        Monitoring.Log.LogGameStartedMessage(botDto);

        GameBoard = GameFactory.Create(botDto.GameBoardFen);
        BoardId = botDto.BoardId;
        Side = (BoardSide)((int)botDto.Side);

        Thread.Sleep(_random.Next(1000, 2000));

        MakeMove(GetNextMove()!.Value, BoardSide.White);
    }

    public void MakeMove(Move? move, BoardSide sideToMove)
    {
        if (move.HasValue && move.Value.IsValidMove() && sideToMove.Equals(Side))
        {
            Monitoring.Log.LogMakeMoveEvent(Id, move.Value);

            LastMoved = DateTime.UtcNow;
            IsInnactive = false;
            _messagePublisher.PublishMoveEvent(BoardId, Id, move.Value);
        }
        /*Thread.Sleep(2100);

        TimeSpan timeSinceLastMove = DateTime.UtcNow - LastMoved;
        if (timeSinceLastMove > TimeSpan.FromSeconds(2))
        {
            Id = Guid.NewGuid();
            Strategy = StrategyFactory.GetRandomStrategy();
            GameBoard.NewGame();
            BoardId = null;
            Side = BoardSide.Undefined;
            IsInnactive = true;

            JoinGame();
        }*/
    }

    public void OnGameEndEvent(Guid winnerGuid)
    {
        string result = "DRAW";
        using var activity = Monitoring.ActivitySource.StartActivity(MethodBase.GetCurrentMethod()!.Name);
        if (Id.Equals(winnerGuid))
        {
            Wins++;
            result = "WIN";
        }
        else if (winnerGuid.Equals(new Guid()))
        {
            Draws++;
        }
        else
        {
            Losses++;
            result = "LOSS";
        }
        Monitoring.Log.LogEndGameEventMessage(Id, winnerGuid, result, BoardId!.Value, GameBoard);

        Id = Guid.NewGuid();
        Strategy = StrategyFactory.GetRandomStrategy();
        GameBoard.NewGame();
        BoardId = null;
        Side = BoardSide.Undefined;
        IsInnactive = true;

        JoinGame();
    }

    public void JoinGame()
    {
        Monitoring.Log.LogJoinGameEventMessage(Id, Wins, Losses, Draws, Strategy.ToString());
        using var activity = Monitoring.ActivitySource.StartActivity(MethodBase.GetCurrentMethod()!.Name);
        Thread.Sleep(_random.Next(6000, 8000));
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
        Monitoring.Log.LogGetNextMoveMessage(Id, Strategy.ToString());
        var move = Strategy.GetNextMove(GameBoard);
        Monitoring.Log.LogRetrievedNextMoveMessage(Id, move);
        return move;
    }

    public void RequestBoardStateUpdate()
    {
        Monitoring.Log.LogInformation("Requesting board state update...");
        if (BoardId.HasValue)
        {
            _messagePublisher.PublishRequestBoardStateUpdate(Id, BoardId.Value);
        }
    }
}
