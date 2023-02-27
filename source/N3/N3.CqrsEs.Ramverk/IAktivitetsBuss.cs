namespace N3.CqrsEs.Ramverk
{
    public interface IAktivitet
    {
        UnikIdentifierare AktivitetsIdentifierare { get; }
    }

    public interface IAktivitetsBuss
    {
        Task Registrera<T>(T data)
            where T : IAktivitet;
    }
}
