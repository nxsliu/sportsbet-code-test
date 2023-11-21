using RacingFeedApi.Events;
using RacingFeedApi.Models;
using RacingFeedApi.Providers;
using RacingFeedApi.Repositories;

namespace RacingFeedApi.Services;

public interface IRaceService
{
    Task CreateRace(RaceCreate race);
}

public class RaceService : IRaceService
{
    private readonly IMessagingProvider _messagingProvider;
    private readonly IRaceRepository _raceRepository;
    private readonly ILogger<RaceService> _logger;

    public RaceService(IMessagingProvider messagingProvider, IRaceRepository raceRepository, ILogger<RaceService> logger)
    {
        _messagingProvider = messagingProvider;
        _raceRepository = raceRepository;
        _logger = logger;
    }

    public async Task CreateRace(RaceCreate race)
    {
        _logger.LogInformation("Creating Race");
        await _messagingProvider.PublishEvent(new RaceCreated { RaceId = race.RaceId });
    }
}