using System.Net.Security;
using RacingFeedApi.Repositories.Entities;

namespace RacingFeedApi.Repositories;

public interface IRaceRepository
{
    Task InsertRace(Race race);
    Task<Race> GetRace(long raceId);
    Task UpdateRace(Race race);
    
}

public class RaceRepository : IRaceRepository
{
    public async Task InsertRace(Race race)
    {
        FakeDataStore.Races.Add(race.RaceId, race);
    }

    public async Task<Race> GetRace(long raceId)
    {
        FakeDataStore.Races.TryGetValue(raceId, out Race race);

        return race;
    }

    public async Task UpdateRace(Race race)
    {
        FakeDataStore.Races[race.RaceId] = race;
    }

}