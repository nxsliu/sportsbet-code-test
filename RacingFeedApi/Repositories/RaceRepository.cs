using RacingFeedApi.Repositories.Entities;

namespace RacingFeedApi.Repositories;

public interface IRaceRepository
{
    Task InsertRace(Race race);
    Task<Race> GetRace(long raceId);
    
}

public class RaceRepository : IRaceRepository
{
    public async Task InsertRace(Race race)
    {
        FakeDataStore.Races.Add(race.RaceId, race);
    }

    public async Task<Race> GetRace(long raceId)
    {
        return FakeDataStore.Races[raceId];
    }
}