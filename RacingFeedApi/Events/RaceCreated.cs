using MediatR;

namespace RacingFeedApi.Events;

public class RaceCreated : INotification
{
    public Guid MessageId { get; set; }
    public Guid CorrolationId { get; set; }
    public long RaceId { get; set; }
}