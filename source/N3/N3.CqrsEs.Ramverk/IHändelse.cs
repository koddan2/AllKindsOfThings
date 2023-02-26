namespace N3.CqrsEs.Ramverk
{
    public interface IHändelse : IMeddelande
    {
        long Revision { get; }
        DateTimeOffset Tidsstämpel { get; }
    }
}
