namespace SharedDTOs.Events;
public class RequestBoardUpdateEvent
{
    public Guid RequesteeId { get; set; }
    public Guid BoardId { get; set; }
}
