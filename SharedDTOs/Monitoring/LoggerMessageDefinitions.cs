using Microsoft.Extensions.Logging;
using SharedDTOs.DTOs;

namespace SharedDTOs.Monitoring;

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