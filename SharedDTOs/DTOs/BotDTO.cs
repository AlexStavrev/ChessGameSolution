using Rudzoft.ChessLib;
namespace SharedDTOs.DTOs
{
    public class BotDTO
    {
        public Guid Id { get; private set; }
        public IGame GameBoard { get; set; }
        public Guid BoardId { get; set; }
        public BoardSide Side { get; set; }
    }

    public enum BoardSide
    {
        Undefined,
        Black,
        White
    }

}
