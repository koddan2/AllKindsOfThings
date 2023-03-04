namespace N3.CqrsEs.Ramverk
{
    public readonly record struct AggregatStrömIdentifierare(
        Type Typ,
        UnikIdentifierare Identifierare
    );

    public readonly record struct AggregatIdentifierareFrånStröm(
        Type Typ,
        string StrömIdentifierare
    );

    public readonly record struct AggregatStrömIdentifierare<T>(UnikIdentifierare Identifierare)
        where T : IAggregatRot;
}
