namespace SharedDTOs.Events;

public class GameEndEvent
{
    public Guid BoardId { get; set; }
    public Guid WinnerId { get; set; }
}
