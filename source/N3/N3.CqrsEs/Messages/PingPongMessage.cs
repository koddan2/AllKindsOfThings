namespace N3.CqrsEs.SkrivModell.JobbPaket
{
    public record ImportAvInkassoÄrendeKölagt(string JobbId);

    public class PingPongMessage
    {
        public string? MessageText { get; set; }
    }
}
