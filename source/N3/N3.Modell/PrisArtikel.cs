namespace N3.Modell
{
    /// <summary>
    /// en artikel definierar ett pris för en tjänst
    /// </summary>
    /// <param name="Namn">Benämningen på artikeln, exv avhysning</param>
    /// <param name="Kod">En kod eller artikelnummer, exv A500</param>
    /// <param name="Pris">Standardpriset för artikeln, exv 5000.- kr</param>
    /// <param name="BokföringsKonto">Bokföringskontot som krediteras när intäkten bokförs.</param>
    /// <param name="Moms">Tillämplig momssats, exv 25 %</param>
    /// <param name="FaktureringsTillfälle">När artikeln ska faktureras, exv direkt eller vid avslut.</param>
    /// <param name="ÄrUtlägg">Indikerar på om artikeln är ett utlägg, vilket innebär ...</param>
    /// <param name="FordringsÄgare">Vem som är slutlig fordringsägare till avgiften, exv ombudet eller kronofogden.</param>
    public readonly record struct PrisArtikel(
        string Namn,
        string Kod,
        Pengar Pris,
        BokföringsKonto BokföringsKonto,
        Moms Moms,
        FaktureringsTillfälle FaktureringsTillfälle,
        bool ÄrUtlägg,
        IndrivningsAgent FordringsÄgare
    );
}
