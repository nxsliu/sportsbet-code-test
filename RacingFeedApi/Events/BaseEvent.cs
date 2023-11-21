using MediatR;

namespace RacingFeedApi.Events;

public class BaseEvent : INotification
{
    public Guid MessageId { get; set; }
    public Guid CorrolationId { get; set; }
    public DateTime PublishedUtc {get;set;}
}