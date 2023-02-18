namespace N3.CqrsEs.Ramverk
{
    public interface IHändelseKassa
    {
        IEnumerable<IHändelse> Hämta(UnikIdentifierare identifierare);
        Task Registrera<T>(AggregatStrömIdentifierare<T> ström, IHändelse händelse);
    }
}
