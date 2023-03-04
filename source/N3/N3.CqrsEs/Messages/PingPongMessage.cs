namespace N3.CqrsEs.Messages
{
    public class ImportAvInkassoÄrendeKölagt
    {
        public string JobbId { get; init; } = "";
    };

    public class PingPongMessage
    {
        public string? MessageText { get; set; }
    }
}
