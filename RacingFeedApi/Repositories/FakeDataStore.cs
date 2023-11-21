using RacingFeedApi.Repositories.Entities;

namespace RacingFeedApi.Repositories;

public static class FakeDataStore
{
    public static Dictionary<long, Race> Races = new Dictionary<long, Race>();
}