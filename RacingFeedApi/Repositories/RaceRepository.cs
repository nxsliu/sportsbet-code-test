using System.Net.Security;
using RacingFeedApi.Repositories.Entities;

namespace RacingFeedApi.Repositories;

public interface IRaceRepository
{
    Task InsertRace(Race race);
    Task<Race> GetRace(long raceId);
    Task UpdateRace(Race race);
    Task<long> GetMaxInternalRaceId();
}

public class RaceRepository : IRaceRepository
{
    public async Task InsertRace(Race race)
    {
        FakeDataStore.Races.Add(race.ExternalRaceId, race);
    }

    public async Task<Race> GetRace(long raceId)
    {
        FakeDataStore.Races.TryGetValue(raceId, out Race race);

        return race;
    }

    public async Task UpdateRace(Race race)
    {
        FakeDataStore.Races[race.ExternalRaceId] = race;
    }

    public async Task<long> GetMaxInternalRaceId()
    {
        if (FakeDataStore.Races.Count == 0)
        {
            return 0;
        }
        
        return FakeDataStore.Races.Keys.Max();
    }
}