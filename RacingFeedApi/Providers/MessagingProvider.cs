using MediatR;

namespace RacingFeedApi.Providers;

public interface IMessagingProvider
{
    Task PublishEvent(INotification notificaiton);
}

public class MessagingProvider : IMessagingProvider
{
    private readonly IMediator _mediator;

    public MessagingProvider(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task PublishEvent(INotification notificaiton)
    {
        await _mediator.Publish(notificaiton);
    }
}