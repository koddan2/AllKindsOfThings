namespace N3.Modell
{
    public readonly record struct BankUppgift(BankKontoTyp Typ, string KontoNummer, string IBAN);
}
