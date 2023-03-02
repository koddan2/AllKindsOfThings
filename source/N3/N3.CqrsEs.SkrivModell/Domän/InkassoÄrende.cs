using N3.CqrsEs.Gemensam.Händelser;
using N3.CqrsEs.Ramverk;
using N3.Modell;
using SmartAnalyzers.CSharpExtensions.Annotations;

namespace N3.CqrsEs.SkrivModell.Domän
{
    [InitRequired]
    public sealed class InkassoÄrende : AbstraktAggregatBasKlass
    {
        private InkassoÄrende()
            : this(UnikIdentifierare.Ingen) { }

        public InkassoÄrende(UnikIdentifierare identifierare)
        {
            Id = identifierare;
        }

        ////private InkassoÄrende Create(object s)
        ////{
        ////    var a = new InkassoÄrende("");
        ////    //a.Applicera(s);
        ////    return a;
        ////}

        public string KlientReferens { get; private set; } = UnikIdentifierare.Ingen;
        public string[] GäldenärsReferenser { get; private set; } = Array.Empty<string>();
        public Faktura[] Fakturor { get; private set; } = Array.Empty<Faktura>();
        public int? ÄrendeNummer { get; private set; }

        public void SkapaÄrende(
            string klientReferens,
            string[] gäldenärsReferenser,
            Faktura[] fakturor
        )
        {
            var händelse = new InkassoÄrendeSkapades(
                Id,
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
                var händelse = new InkassoÄrendeBlevTilldelatÄrendeNummer(Id, ärendeNummer);
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

#if DEBUG
        public void _Applicera(object e)
        {
            if (e is InkassoÄrendeSkapades e0)
            {
                Applicera(e0);
            }
            else if (e is InkassoÄrendeBlevTilldelatÄrendeNummer e1)
            {
                Applicera(e1);
            }
        }
#endif
    }
}
