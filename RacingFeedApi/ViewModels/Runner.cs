namespace RacingFeedApi.ViewModels;

public class Runner
{
    public long Id { get; set; }
    public int TabNo { get; set; }
    public int Barrier { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    public string Jockey { get; set; }
    public string Trainer { get; set; }
}