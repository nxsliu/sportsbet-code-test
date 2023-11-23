namespace RacingFeedApi.Repositories.Entities;

public class Race
{
    public long ExternalRaceId { get; set; }
    public long InternalRaceId { get; set; }
    public string RaceDetails { get; set; }
    public DateTime UpdatedUtc { get; set; }
}