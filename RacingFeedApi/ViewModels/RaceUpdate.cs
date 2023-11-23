using System.Xml.Serialization;

namespace RacingFeedApi.ViewModels;

[XmlRoot("RaceUpdate")]
public class RaceUpdate
{
    public long MeetingId { get; set; }
    public long RaceId { get; set; }
    public string RaceLocation { get; set; }
    public int RaceDistance { get; set; }
    public int RaceNo { get; set; }
    public string RaceType { get; set; }
    public string RaceInfo { get; set; }
    public string TrackCondition { get; set; }
    public string Source { get; set; }
    public string PriceType { get; set; }
    public int PoolSize { get; set; }
    public long StartTime { get; set; }
    public long CreationTime { get; set; }

    [XmlArray("Runners")]
    [XmlArrayItem("Runner")]
    public List<Runner> Runners { get; set; }
}