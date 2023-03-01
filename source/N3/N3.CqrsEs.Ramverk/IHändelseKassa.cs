namespace N3.CqrsEs.Ramverk
{
    public enum HändelseModus
    {
        LäggTill = 0,
        SkapaNy,
        LäggTillOptimistiskKontroll,
    }

    public interface IHändelseRegistrator
    {
        Task Registrera<T>(
            AggregatStrömIdentifierare<T> ström,
            IHändelse händelse,
            HändelseModus modus
        )
            where T : IAggregatBas;
    }

    public interface IHändelseKassa : IHändelseRegistrator
    {
        Task<IEnumerable<IHändelse>> Hämta<T>(AggregatStrömIdentifierare<T> ström)
            where T : IAggregatBas;
    }
}
