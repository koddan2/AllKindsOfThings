using N3.CqrsEs.Gemensam.Händelser;
using N3.CqrsEs.Ramverk;
using N3.Modell;

namespace N3.CqrsEs.SkrivModell.Domän
{
    public sealed class InkassoÄrende : IAggregatBas
    {
        private InkassoÄrende() { }

        public InkassoÄrende(
            UnikIdentifierare identifierare)
        {
            Identifierare = identifierare;
        }

        public UnikIdentifierare Identifierare { get; }

        public async Task SkapaÄrende(
            IHändelseKassa händelseKassa,
            UnikIdentifierare klientReferens,
            UnikIdentifierare[] gäldenärsReferenser,
            Faktura[] fakturor,
            long ärendeNummer)
        {
            var händelse = new InkassoÄrendeSkapades(Identifierare, klientReferens, gäldenärsReferenser, fakturor, ärendeNummer);
            await händelseKassa.Registrera(this.TillStrömIdentifierare(), händelse);
        }
    }
}