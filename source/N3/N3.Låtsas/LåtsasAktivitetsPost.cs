////using SAK;

using SmartAnalyzers.CSharpExtensions.Annotations;

namespace N3.Låtsas
{
    [InitRequired]
    public class LåtsasAktivitetsPost
    {
        public string Id => AktivitetsIdentifierare;
        public string AktivitetsIdentifierare { get; set; }
        public string? Kategori { get; set; }
        public string? TypNamn { get; set; }
        public string? JsonData { get; set; }
        public DateTimeOffset? ReservationsTidsstämpel { get; set; }
        public string? ReservationsIdentifierare { get; set; }
    }
}
