namespace RacingFeedApi.DomainModels;

public class Race
{
    public long RaceId { get; set; }
    public string RaceLocation { get; set; }
    public int Distance { get; set; }
    public int RaceNumber { get; set; }
    public string RaceType { get; set; }
    public string RaceInfo { get; set; }
    public string TrackCondition { get; set; }
    public DateTime StartTimeUtc { get; set; }
    public IList<Runner> Runners { get; set; }
}