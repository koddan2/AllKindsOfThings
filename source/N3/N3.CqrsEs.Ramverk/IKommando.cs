namespace N3.CqrsEs.Ramverk
{
    public interface IKommando : IMeddelande
    {
        string Auktorisering { get; }
        long FörväntadRevision { get; }
    }
}
