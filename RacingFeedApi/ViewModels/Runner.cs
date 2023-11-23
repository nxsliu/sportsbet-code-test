using System.Xml.Serialization;

namespace RacingFeedApi.ViewModels;

[XmlRoot("Runner")]
public class Runner
{
    [XmlAttribute]
    public long Id { get; set; }

    [XmlAttribute]
    public int TabNo { get; set; }

    [XmlAttribute]
    public int Barrier { get; set; }

    [XmlAttribute]
    public string Name { get; set; }

    [XmlAttribute]
    public decimal Price { get; set; }

    [XmlAttribute]
    public string Jockey { get; set; }

    [XmlAttribute]
    public string Trainer { get; set; }
}