namespace N3.CqrsEs.Ramverk
{
    public interface IMeddelande
    {
        UnikIdentifierare KorrelationsIdentifierare { get; }

        IEnumerable<string>? Historia { get; set; }
    }
}
