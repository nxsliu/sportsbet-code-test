namespace RacingFeedApi.Repositories.Entities;

public class Race
{
    public long RaceId { get; set; }
    public string RaceDetails { get; set; }
    public DateTime UpdatedUtc { get; set; }
}