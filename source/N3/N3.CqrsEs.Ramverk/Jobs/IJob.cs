namespace N3.CqrsEs.Ramverk.Jobs
{
    public interface IJob
    {
        string Id { get; }
        string? ReservationsId { get; set; }
        DateTimeOffset? ReservationsTidsstämpel { get; set; }
    }
}
