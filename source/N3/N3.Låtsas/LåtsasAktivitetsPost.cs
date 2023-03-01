////using SAK;

namespace N3.Låtsas
{
    public record LåtsasAktivitetsPost
    {
        public long? Id { get; set; }
        public string? AktivitetsIdentifierare { get; internal set; }
        public string? Kategori { get; internal set; }
        public string? TypNamn { get; internal set; }
        public string? JsonData { get; internal set; }
        public string? ReservationsTidsstämpel { get; internal set; }
        public string? ReservationsIdentifierare { get; internal set; }
    }
}
