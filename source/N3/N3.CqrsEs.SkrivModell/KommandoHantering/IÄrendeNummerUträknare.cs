namespace N3.CqrsEs.SkrivModell.KommandoHantering
{
    public interface IÄrendeNummerUträknare
    {
        Task<long> TaFramNästaLedigaÄrendeNummer();
    }
}
