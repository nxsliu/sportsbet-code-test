using RacingFeedApi.Events;
using RacingFeedApi.Exceptions;
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
        _logger.LogInformation("Creating Race {raceId}", race.RaceId);

        if (await _raceRepository.GetRace(race.RaceId) != null)
        {
            throw new CreateResourceException("RaceId already exists");
        }

        var raceCreated = new Repositories.Entities.Race
        {
            RaceId = race.RaceId,
            RaceLocation = race.RaceLocation,
            Distance = race.RaceDistance,
            RaceNumber = race.RaceNo,
            RaceType = race.RaceType,
            RaceInfo = race.RaceInfo,
            TrackCondition = race.TrackCondition,
            StartTimeUtc = DateTimeOffset.FromUnixTimeSeconds(race.StartTime).UtcDateTime,
            Runners = race.Runners.Select(r => new Repositories.Entities.Runner
            {
                Id = r.Id,
                Number = r.TabNo,
                Barrier = r.Barrier,
                Name = r.Name,
                WinPrice = r.Price,
                Jockey = r.Jockey,
                Trainer = r.Trainer
            }).ToList(),
            UpdatedUtc = DateTime.UtcNow
        };
        
        await _raceRepository.InsertRace(raceCreated);

        //await _messagingProvider.PublishEvent(new RaceCreated { RaceId = race.RaceId });
    }
}