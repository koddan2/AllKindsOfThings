using SmartAnalyzers.CSharpExtensions.Annotations;

namespace N3.CqrsEs.Ramverk.Jobs
{
    [InitRequired]
    public abstract class AbstractJob : IJob
    {
        public string Id { get; set; }
        public string? ReservationsId { get; set; }
        public DateTimeOffset? ReservationsTidsstämpel { get; set; }
    }
}
