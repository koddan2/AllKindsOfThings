using SmartAnalyzers.CSharpExtensions.Annotations;

namespace N3.CqrsEs.Ramverk
{
    public interface IJobb
    {
        string Id { get; }
        string? ReservationsId { get; set; }
        DateTimeOffset? ReservationsTidsstämpel { get; set; }
    }

    [InitRequired]
    public abstract class AbstraktJobb : IJobb
    {
        public string Id { get; set; }
        public string? ReservationsId { get; set; }
        public DateTimeOffset? ReservationsTidsstämpel { get; set; }
    }

    public record JobbStatus(string JobbId, bool Reserverad);

    public record ReservationsKvitto(string AktivitetsId, string? ReservationsId);

    public enum JobbStatusFiltrering
    {
        EjAvslutade = 0,
        EjReserverade,
        Alla,
    }

    public interface IJobbKö
    {
        Task<ReservationsKvitto> Reservera<T>(string jobbId)
            where T : IJobb;

        Task TaBort<T>(string jobbId, ReservationsKvitto kvitto)
            where T : IJobb;

        Task<IEnumerable<JobbStatus>> HämtaStatus<T>(
            JobbStatusFiltrering filtrering = JobbStatusFiltrering.EjAvslutade
        )
            where T : IJobb;

        Task<JobbStatus> HämtaStatus<T>(string jobbId)
            where T : IJobb;

        Task Kölägg<T>(T jobb)
            where T : IJobb;
    }
}
