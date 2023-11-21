namespace RacingFeedApi.DomainModels;

public class Runner
{
    public long Id { get; set; }
    public int Number { get; set; }
    public int Barrier { get; set; }
    public string Name { get; set; }
    public decimal WinPrice { get; set; }
    public string Jockey { get; set; }
    public string Trainer { get; set; }
}