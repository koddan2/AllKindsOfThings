namespace N3.CqrsEs.Ramverk
{
    public static class HändelseKassaFörlängningar
    {
        public static AggregatStrömIdentifierare<T> TillStrömIdentifierare<T>(this T aggregat)
            where T : IAggregatRot
        {
            return new AggregatStrömIdentifierare<T>(aggregat.Id);
        }

        public static string ByggStrömIdentifierare(this AggregatStrömIdentifierare data)
        {
            return $"{data.Typ.Name}-{data.Identifierare}";
        }

        public static string TillIdentifierare(this AggregatIdentifierareFrånStröm data)
        {
            var parts = data.StrömIdentifierare.Split(new[] { "-" }, StringSplitOptions.None);
            return parts[1];
        }

        public static string ByggStrömIdentifierare<T>(this AggregatStrömIdentifierare<T> data)
            where T : IAggregatRot
        {
            return $"{typeof(T).Name}-{data.Identifierare}";
        }
    }
}
