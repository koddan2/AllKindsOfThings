namespace N3.CqrsEs.Ramverk
{
    public interface IAggregatHändelse : IHändelse
    {
        public string AggregatIdentifierare { get; }
    }
}
