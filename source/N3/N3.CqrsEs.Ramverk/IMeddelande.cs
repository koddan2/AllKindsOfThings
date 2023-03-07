namespace N3.CqrsEs.Ramverk
{
    public interface IMeddelande
    {
        /// <summary>
        /// En identifierare som är unik för specifickt detta meddelande. Kan användas som korrelation för att spåra meddelandets rutt.
        /// </summary>
        string KorrelationsIdentifierare { get; }

        /// <summary>
        /// Detta är en sekvens med strängar som bör motsvara systembenämningar genom vilka detta meddelande har ruttats.
        /// </summary>
        ////IEnumerable<string> Historia { get; }
    }
}
