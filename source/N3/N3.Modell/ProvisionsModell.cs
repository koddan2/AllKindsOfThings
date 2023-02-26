namespace N3.Modell
{
    //
    // En provisionsmodell är en tabell där olika skuldelement har respektive provisionsinställningar
    public readonly record struct ProvisionsModell(
        IDictionary<SkuldElement, ProvisionsInställning> Inställningar
    );
}
