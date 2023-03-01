namespace N3.CqrsEs.Ramverk
{
    public interface IAktivitet
    {
        UnikIdentifierare AktivitetsIdentifierare { get; }
        AktivitetsKategori AktivitetsKategori { get; }
    }

    public record AktivitetsKategori(string Namn);

    public record AktivitetsStatus(
        UnikIdentifierare AktivitetsIdentifierare,
        AktivitetsKategori AktivitetsKategori,
        bool Reserverad
    );

    public record AktivitetsKvitto(
        UnikIdentifierare AktivitetsIdentifierare,
        UnikIdentifierare? ReservationsIdentifierare
    );

    public enum AktivitetsStatusFiltrering
    {
        EjAvslutade = 0,
        EjReserverade,
        Alla,
    }

    public interface IAktivitetsBuss
    {
        Task<AktivitetsKvitto> Reservera(UnikIdentifierare aktivitetsIdentifierare);
        Task<IEnumerable<AktivitetsStatus>> HämtaStatus(
            AktivitetsKategori kategori,
            AktivitetsStatusFiltrering filtrering = AktivitetsStatusFiltrering.EjAvslutade
        );
        Task<AktivitetsStatus> HämtaStatus(UnikIdentifierare aktivitetsIdentifierare);
        Task Kölägg<T>(T data)
            where T : IAktivitet;
    }
}
