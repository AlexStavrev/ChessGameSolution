using Microsoft.Extensions.Logging;
using Rudzoft.ChessLib;
using Rudzoft.ChessLib.Enums;
using Rudzoft.ChessLib.Types;
using SharedDTOs.DTOs;
using SharedDTOs.Events;

namespace SharedDTOs.Monitoring;


// Game Manager definitions
public static partial class LoggerMessageDefinitions_GameManager
{
    [LoggerMessage(EventId = 0, Level = LogLevel.Information,
        Message = "Player joined queue: {PlayerId}"
    )]
    public static partial void LogPlayerJoinQueueMessage(this ILogger logger, Guid playerId);

    [LoggerMessage(EventId = 1, Level = LogLevel.Information,
        Message = "Board joined queue: {BoardId}"
    )]
    public static partial void LogBoardJoinQueueMessage(this ILogger logger, Guid boardId);

    [LoggerMessage(EventId = 2, Level = LogLevel.Information,
        Message = "Created a game on board {BoardId} for players {Players}"
    )]
    public static partial void LogGameCreatedMessage(this ILogger logger, Guid boardId, ICollection<BotDTO> players);

    [LoggerMessage(EventId = 3, Level = LogLevel.Information,
        Message = "Checking if a game can be made; Current players: {PlayerCount}; Current boards: {BoardCount}"
    )]
    public static partial void LogTryingToMakeAGameMessage(this ILogger logger, int playerCount, int boardCount);
}

public static partial class LoggerMessageDefinitionsWarnings_GameManager
{
    [LoggerMessage(EventId = 0, Level = LogLevel.Warning,
        Message = "Board is already in queue but tried to join! Id: {BoardId}"
    )]
    public static partial void LogBoardAlreadyInQueueWarning(this ILogger logger, Guid boardId);

    [LoggerMessage(EventId = 1, Level = LogLevel.Warning,
        Message = "Player is already in queue but tried to join! Id: {PlayerId}"
    )]
    public static partial void LogPlayerAlreadyInQueueWarning(this ILogger logger, Guid playerId);
}


// Board Manager Definitions
public static partial class LoggerMessageDefinitions_BoardManager
{
    [LoggerMessage(EventId = 0, Level = LogLevel.Information,
        Message = "Board {BoardId}: Starting a game with {Bots}"
    )]
    public static partial void LogGameStartEvent(this ILogger logger, ICollection<BotDTO> bots, Guid boardId);

    [LoggerMessage(EventId = 1, Level = LogLevel.Information,
        Message = "Board {BoardId}: Ending game for {Bots} - Winner: {winnerId}; End game type: {GameEndType}; Fen: {Fen}"
    )]
    public static partial void LogEndGameEvent(this ILogger logger, ICollection<BotDTO> bots, Guid winnerId, Guid boardId, GameEndTypes gameEndType, string fen);

    [LoggerMessage(EventId = 2, Level = LogLevel.Information,
        Message = "Board {BoardId}: Updating board state; Fen: {Fen}"
    )]
    public static partial void LogUpdateBoardStateMessage(this ILogger logger, Guid boardId, string fen);
    
    [LoggerMessage(EventId = 3, Level = LogLevel.Information,
        Message = "Handling player move {Move} by {PlayerId}"
    )]
    public static partial void LogPlayerMoveMessage(this ILogger logger, Guid playerId, Move move);
}
public static partial class LoggerMessageDefinitionsWarnings_BoardManager
{
    [LoggerMessage(EventId = 0, Level = LogLevel.Warning,
        Message = "Player {PlayerId} is not in turn to move! It is {SideToMove}'s turn!"
    )]
    public static partial void LogWrongSideToMoveWarning(this ILogger logger, Guid playerId, BoardSide sideToMove);

    [LoggerMessage(EventId = 1, Level = LogLevel.Warning,
        Message = "Player {PlayerId} tried to make an invalid move {InvalidMove} on {GameBoard}!"
    )]
    public static partial void LogInvalidMoveWarning(this ILogger logger, Guid playerId, Move invalidMove, IGame gameBoard);
}

// BotAI Definitions
public static partial class LoggerMessageDefinitions_BotAI
{
    [LoggerMessage(EventId = 0, Level = LogLevel.Information,
        Message = "Player joined queue: {PlayerId}"
    )]
    public static partial void LogPlayerJoinQueueMessage(this ILogger logger, Guid playerId);
    
    [LoggerMessage(EventId = 1, Level = LogLevel.Information,
        Message = "Board Updated: {BoardFenState}"
    )]
    public static partial void LogBoardStateUpdateMessage(this ILogger logger, string boardFenState);

    [LoggerMessage(EventId = 2, Level = LogLevel.Information,
        Message = "Game started: Bot state: {Bot}"
    )]
    public static partial void LogGameStartedMessage(this ILogger logger, BotDTO bot);

    [LoggerMessage(EventId = 3, Level = LogLevel.Information,
       Message = "{BotId}: Making move: {Move}"
   )]
    public static partial void LogMakeMoveEvent(this ILogger logger, Guid botId, Move move);

    [LoggerMessage(EventId = 4, Level = LogLevel.Information,
       Message = "{BotId}: Game ended; {Result}; WinnerId: {WinnerId} on board {BoardId}. Game board state: {GameBoard}."
    )]
    public static partial void LogEndGameEventMessage(this ILogger logger, Guid botId, Guid winnerId, string result, Guid boardId, IGame gameBoard);

    [LoggerMessage(EventId = 5, Level = LogLevel.Information,
       Message = "{BotId}: Joining queue; Wins: {Wins}, Losses: {Losses}, Draws: {Draws}; Strategy: {Strategy}."
    )]
    public static partial void LogJoinGameEventMessage(this ILogger logger, Guid botId, int Wins, int Losses, int Draws, string strategy);

    [LoggerMessage(EventId = 6, Level = LogLevel.Information,
       Message = "{BotId}: Getting next move with strategy {Strategy}."
    )]
    public static partial void LogGetNextMoveMessage(this ILogger logger, Guid botId, string strategy);

    [LoggerMessage(EventId = 7, Level = LogLevel.Information,
   Message = "{BotId}: Retrieved move {Move}."
)]
    public static partial void LogRetrievedNextMoveMessage(this ILogger logger, Guid botId, Move? move);
}
public static partial class LoggerMessageDefinitionsWarnings_BotAI
{
    [LoggerMessage(EventId = 0, Level = LogLevel.Warning,
        Message = "Bot {botId} is not part of the game in game start event {GameStartEvent}"
    )]
    public static partial void LogBotNotInStartGameEventWarning(this ILogger logger, Guid botId, GameStartEvent gameStartEvent);

    [LoggerMessage(EventId = 1, Level = LogLevel.Warning,
        Message = "Error while getting next move using {MoveType}; Error message was: {ExceptionMessage}"
    )]
    public static partial void LogGetNextMoveStrategyErrorWarning(this ILogger logger, MoveGenerationType moveType, string exceptionMessage);
}