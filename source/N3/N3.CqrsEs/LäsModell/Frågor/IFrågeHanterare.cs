namespace N3.CqrsEs.LäsModell.Frågor
{
    public interface IFrågeHanterare<TQuery, TResult>
    {
        ////Task<InkassoÄrendeFullVyModell> Hantera(HämtaSpecifiktInkassoÄrende fråga);
        Task<TResult> Hantera(TQuery query);
    }
}
