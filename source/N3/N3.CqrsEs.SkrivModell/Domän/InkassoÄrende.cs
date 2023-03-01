using N3.CqrsEs.Gemensam.Händelser;
using N3.CqrsEs.Ramverk;
using N3.Modell;
using SmartAnalyzers.CSharpExtensions.Annotations;

namespace N3.CqrsEs.SkrivModell.Domän
{
    [InitRequired]
    public sealed class InkassoÄrende : AbstraktAggregatBasKlass<InkassoÄrende>
    {
        public InkassoÄrende()
            : base() { }
        public InkassoÄrende(UnikIdentifierare identifierare)
            : base(identifierare)
        {
            ////Identifierare = identifierare;
        }

        public override string Id => this
            .TillStrömIdentifierare()
            .ByggStrömIdentifierare();

        ////public UnikIdentifierare Identifierare { get; }
        public long Revision { get; private set; }

        public UnikIdentifierare KlientReferens { get; private set; } = UnikIdentifierare.Ingen;
        public UnikIdentifierare[] GäldenärsReferenser { get; private set; } =
            Array.Empty<UnikIdentifierare>();
        public Faktura[] Fakturor { get; private set; } = Array.Empty<Faktura>();
        public int? ÄrendeNummer { get; private set; }

        public void SkapaÄrende(
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

            Applicera(händelse);
            AddUncommittedEvent(händelse);
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

        public void TilldelaÄrendeNummer(int ärendeNummer)
        {
            if (ÄrendeNummer is null)
            {
                var händelse = new InkassoÄrendeBlevTilldelatÄrendeNummer(
                    Identifierare,
                    ärendeNummer
                );
                Applicera(händelse);
                AddUncommittedEvent(händelse);
            }
            else
            {
                throw new InvalidOperationException(
                    $"Värdet {ärendeNummer} är inte ett giltigt ärendenummer."
                );
            }
        }

        private void Applicera(InkassoÄrendeSkapades händelse)
        {
            this.GäldenärsReferenser = händelse.GäldenärsReferenser;
            this.Fakturor = händelse.Fakturor;
            this.KlientReferens = händelse.KlientReferens;
            this.Version++;
        }

        private void Applicera(InkassoÄrendeBlevTilldelatÄrendeNummer händelse)
        {
            this.ÄrendeNummer = händelse.ÄrendeNummer;
            this.Version++;
        }
    }
}
