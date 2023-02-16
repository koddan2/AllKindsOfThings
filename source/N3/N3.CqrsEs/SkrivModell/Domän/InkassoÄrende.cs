using Cqrs.Domain;
using N3.CqrsEs.Händelser;
using N3.Modell;

namespace N3.CqrsEs.SkrivModell.Domän
{
    public sealed class InkassoÄrende : AggregateRoot<string>
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
            var @event = new InkassoÄrendeSkapades(identifierare, klientReferens, gäldenärsReferenser, fakturor, ärendeNummer);
            ApplyChange(@event);
        }
    }
}