using System.Security;
using MediatR;
using RacingFeedApi.Events;

namespace RacingFeedApi.IntegrationTests.Fakes;

public class FakeRaceCreatedHandler : INotificationHandler<RaceCreated>
{
    public List<RaceCreated> HandledEvents { get; set; }

    public FakeRaceCreatedHandler()
    {
        HandledEvents = new List<RaceCreated>();
    }

    public Task Handle(RaceCreated notification, CancellationToken cancellationToken)
    {
        HandledEvents.Add(notification);

        return Task.CompletedTask;
    }
}