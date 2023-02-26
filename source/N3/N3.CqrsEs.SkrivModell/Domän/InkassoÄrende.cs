using N3.CqrsEs.Gemensam.Händelser;
using N3.CqrsEs.Ramverk;
using N3.Modell;

namespace N3.CqrsEs.SkrivModell.Domän
{
    public sealed class InkassoÄrende : IAggregatBas
    {
        private InkassoÄrende() { }

        public InkassoÄrende(UnikIdentifierare identifierare)
        {
            Identifierare = identifierare;
        }

        public UnikIdentifierare Identifierare { get; }

        public async Task SkapaÄrende(
            IHändelseRegistrator händelseRegistrator,
            UnikIdentifierare klientReferens,
            UnikIdentifierare[] gäldenärsReferenser,
            Faktura[] fakturor,
            long ärendeNummer
        )
        {
            var händelse = new InkassoÄrendeSkapades(
                Identifierare,
                klientReferens,
                gäldenärsReferenser,
                fakturor,
                ärendeNummer
            );
            await händelseRegistrator.Registrera(this.TillStrömIdentifierare(), händelse);
        }
    }
}
