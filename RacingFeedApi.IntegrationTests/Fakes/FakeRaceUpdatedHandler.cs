using MediatR;
using RacingFeedApi.Events;

namespace RacingFeedApi.IntegrationTests.Fakes;

public class FakeRaceUpdatedHandler : INotificationHandler<RaceUpdated>
{
    public Task Handle(RaceUpdated notification, CancellationToken cancellationToken)
    {
        MessagesHandledHelper.RaceUpdatedMessagedHandled.Add(notification);

        return Task.CompletedTask;
    }
}