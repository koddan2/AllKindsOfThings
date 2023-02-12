using CQRSlite.Domain;
using N3.CqrsEs.Händelser;
using N3.Modell;

namespace N3.CqrsEs.SkrivModell.Domän
{
    public sealed class InkassoÄrende : AggregateRoot
    {
        private InkassoÄrende() { }

        public InkassoÄrende(
            UnikIdentifierare identifierare,
            UnikIdentifierare klientReferens,
            UnikIdentifierare[] gäldenärsReferenser,
            Faktura[] fakturor,
            long ärendeNummer)
        {
            Id = identifierare;
            ApplyChange(new InkassoÄrendeSkapades(identifierare, klientReferens, gäldenärsReferenser, fakturor, ärendeNummer));
        }
    }
}