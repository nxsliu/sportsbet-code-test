using System.Text.Json;
using RacingFeedApi.Exceptions;
using RacingFeedApi.ViewModels;
using RacingFeedApi.Providers;
using RacingFeedApi.Repositories;
using RacingFeedApi.Events;
using RacingFeedApi.DomainModels;

namespace RacingFeedApi.Services;

public interface IRaceService
{
    Task CreateRace(RaceCreate race);
    Task UpdateRace(RaceUpdate race);
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

        try
        {
            Race raceCreated = new Race
            {
                RaceId = race.RaceId,
                RaceLocation = race.RaceLocation,
                Distance = race.RaceDistance,
                RaceNumber = race.RaceNo,
                RaceType = race.RaceType,
                RaceInfo = race.RaceInfo,
                TrackCondition = race.TrackCondition,
                StartTimeUtc = DateTimeOffset.FromUnixTimeSeconds(race.StartTime).UtcDateTime,
                Runners = race.Runners.Select(r => new DomainModels.Runner
                {
                    Id = r.Id,
                    Number = r.TabNo,
                    Barrier = r.Barrier,
                    Name = r.Name,
                    WinPrice = r.Price,
                    Jockey = r.Jockey,
                    Trainer = r.Trainer
                }).ToList(),
            };

            _logger.LogInformation("Race {raceId} successfully mapped to domain", race.RaceId);

            await _raceRepository.InsertRace(new Repositories.Entities.Race
            {
                RaceId = raceCreated.RaceId,
                RaceDetails = JsonSerializer.Serialize(raceCreated),
                UpdatedUtc = DateTime.UtcNow
            });

            _logger.LogInformation("Race {raceId} inserted into DB", race.RaceId);

            await _messagingProvider.PublishEvent(new RaceCreated
            {
                MessageId = Guid.NewGuid(),
                CorrolationId = Guid.NewGuid(),
                Message = JsonSerializer.Serialize(raceCreated),
                PublishedUtc = DateTime.UtcNow
            });

            _logger.LogInformation("Race {raceId} RaceCreated event published", race.RaceId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating raceId {raceId}", race.RaceId);

            throw new CreateResourceException("An exception occurred while creating this race");
        }
    }

    public async Task UpdateRace(RaceUpdate race)
    {
        _logger.LogInformation("Creating Race {raceId}", race.RaceId);

        var raceToUpdate = await _raceRepository.GetRace(race.RaceId);
        if (raceToUpdate == null)
        {
            throw new ResourceNotFoundException("RaceId not found");
        }

        try
        {
            Race raceUpdated = new Race
            {
                RaceId = race.RaceId,
                RaceLocation = race.RaceLocation,
                Distance = race.RaceDistance,
                RaceNumber = race.RaceNo,
                RaceType = race.RaceType,
                RaceInfo = race.RaceInfo,
                TrackCondition = race.TrackCondition,
                StartTimeUtc = DateTimeOffset.FromUnixTimeSeconds(race.StartTime).UtcDateTime,
                Runners = race.Runners.Select(r => new DomainModels.Runner
                {
                    Id = r.Id,
                    Number = r.TabNo,
                    Barrier = r.Barrier,
                    Name = r.Name,
                    WinPrice = r.Price,
                    Jockey = r.Jockey,
                    Trainer = r.Trainer
                }).ToList(),
            };

            _logger.LogInformation("Race {raceId} successfully mapped to domain", race.RaceId);

            var raceOptomisticConcurrencyCheck = await _raceRepository.GetRace(race.RaceId);
            if (raceToUpdate.UpdatedUtc != raceOptomisticConcurrencyCheck.UpdatedUtc)
            {
                throw new UpdateResourceException("Race updated failed due to concurrent update");
            }

            await _raceRepository.UpdateRace(new Repositories.Entities.Race
            {
                RaceId = raceUpdated.RaceId,
                RaceDetails = JsonSerializer.Serialize(raceUpdated),
                UpdatedUtc = DateTime.UtcNow
            });

            _logger.LogInformation("Race {raceId} updated in DB", race.RaceId);

            await _messagingProvider.PublishEvent(new RaceUpdated
            {
                MessageId = Guid.NewGuid(),
                CorrolationId = Guid.NewGuid(),
                Message = JsonSerializer.Serialize(raceUpdated),
                PublishedUtc = DateTime.UtcNow
            });

            _logger.LogInformation("Race {raceId} RaceUpdated event published", race.RaceId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating raceId {raceId}", race.RaceId);

            throw new UpdateResourceException("An exception occurred while updating this race");
        }
    }
}