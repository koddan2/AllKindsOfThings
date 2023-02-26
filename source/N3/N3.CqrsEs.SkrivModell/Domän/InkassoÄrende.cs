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
        public long Revision { get; private set; }

        public UnikIdentifierare KlientReferens { get; private set; } = UnikIdentifierare.Ingen;
        public UnikIdentifierare[] GäldenärsReferenser { get; private set; } =
            Array.Empty<UnikIdentifierare>();
        public Faktura[] Fakturor { get; private set; } = Array.Empty<Faktura>();
        public int? ÄrendeNummer { get; private set; }

        public async Task SkapaÄrende(
            IHändelseRegistrator händelseRegistrator,
            UnikIdentifierare klientReferens,
            UnikIdentifierare[] gäldenärsReferenser,
            Faktura[] fakturor
        )
        {
            var händelse = new InkassoÄrendeSkapades(
                Identifierare,
                klientReferens,
                gäldenärsReferenser,
                fakturor
            );
            await händelseRegistrator.Registrera(this.TillStrömIdentifierare(), händelse);
        }

        internal void Ladda(IEnumerable<IHändelse> händelser)
        {
            foreach (var händelse in händelser)
            {
                if (händelse is InkassoÄrendeSkapades skapades)
                {
                    Revision = skapades.Revision;
                    KlientReferens = skapades.KlientReferens;
                    GäldenärsReferenser = skapades.GäldenärsReferenser;
                    Fakturor = skapades.Fakturor;
                }
                else if (händelse is InkassoÄrendeBlevTilldelatÄrendeNummer fickÄrendeNr)
                {
                    ÄrendeNummer = fickÄrendeNr.ÄrendeNummer;
                }
            }
        }

        internal async Task TilldelaÄrendeNummer(
            IHändelseRegistrator händelseRegistrator,
            int ärendeNummer
        )
        {
            if (ÄrendeNummer is null)
            {
                var händelse = new InkassoÄrendeBlevTilldelatÄrendeNummer(
                    Identifierare,
                    ärendeNummer
                );
                await händelseRegistrator.Registrera(
                    new AggregatStrömIdentifierare<InkassoÄrende>(Identifierare),
                    händelse
                );
            }
        }
    }
}
