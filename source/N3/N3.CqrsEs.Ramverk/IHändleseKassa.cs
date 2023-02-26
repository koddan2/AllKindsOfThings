namespace N3.CqrsEs.Ramverk
{
    public interface IHändelseRegistrator
    {
        Task Registrera<T>(AggregatStrömIdentifierare<T> ström, IHändelse händelse);
    }

    public interface IHändelseKassa : IHändelseRegistrator
    {
        Task<IEnumerable<IHändelse>> Hämta<T>(AggregatStrömIdentifierare<T> ström);
    }
}
