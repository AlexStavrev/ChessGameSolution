using Rudzoft.ChessLib;

namespace SharedDTOs.DTOs;

public class BotDTO
{
    public Guid Id { get; set; }
    public string GameBoardFen { get; set; }
    public Guid? BoardId { get; set; }
    public BoardSide Side { get; set; }
}
