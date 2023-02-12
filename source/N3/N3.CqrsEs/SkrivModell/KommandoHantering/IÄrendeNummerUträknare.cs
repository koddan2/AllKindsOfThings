namespace N3.CqrsEs.SkrivModell.Hantering
{
    public interface IÄrendeNummerUträknare
    {
        Task<long> TaFramNästaLedigaÄrendeNummer();
    }
}
