using N3.CqrsEs.Ramverk;

namespace N3.CqrsEs.LäsModell.Frågor
{
    public interface IFrågeHanterare<TFråga, TResultat>
        where TFråga : IMeddelande
    {
        Task<TResultat> Hantera(TFråga fråga, CancellationToken cancellationToken = default);
    }
}
