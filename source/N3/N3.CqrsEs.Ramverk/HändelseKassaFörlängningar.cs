namespace N3.CqrsEs.Ramverk
{
    public static class HändelseKassaFörlängningar
    {
        public static AggregatStrömIdentifierare<T> TillStrömIdentifierare<T>(this T aggregat)
            where T : IAggregatBas
        {
            return new AggregatStrömIdentifierare<T>(aggregat.Identifierare);
        }

        public static string ByggStrömIdentifierare<T>(this AggregatStrömIdentifierare<T> data)
        {
            return $"{typeof(T).Name}-{data.Identifierare}";
        }
    }
}
