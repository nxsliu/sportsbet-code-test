using MediatR;
using RacingFeedApi.Events;

namespace RacingFeedApi.IntegrationTests.Fakes;

public class FakeRaceCreatedHandler : INotificationHandler<RaceCreated>
{
    public Task Handle(RaceCreated notification, CancellationToken cancellationToken)
    {
        MessagesHandledHelper.RaceCreatedMessagedHandled.Add(notification);

        return Task.CompletedTask;
    }
}