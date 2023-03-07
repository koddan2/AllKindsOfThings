using N3.CqrsEs.Gemensam.Händelser;
using N3.CqrsEs.Ramverk;
using N3.Modell;
using SmartAnalyzers.CSharpExtensions.Annotations;

namespace N3.CqrsEs.SkrivModell.Domän
{
    [InitRequired]
    public sealed class InkassoKlient : AbstraktAggregatRotBasKlass
    {
        public InkassoKlient(UnikIdentifierare identifierare)
        {
            Id = identifierare;
        }

        public string? FullkomligtNamn { get; private set; }

        public void SkapaKlient(string FullkomligtNamn)
        {
            var händelse = new InkassoKlientSkapades(Id, FullkomligtNamn);
            Applicera(händelse);
            AddUncommittedEvent(händelse);
        }

        private void Applicera(InkassoKlientSkapades händelse)
        {
            this.FullkomligtNamn = händelse.FullkomligtNamn;
        }

        ////internal void Ladda(IEnumerable<IHändelse> händelser)
        ////{
        ////    foreach (var händelse in händelser)
        ////    {
        ////        if (händelse is InkassoKlientSkapades skapades)
        ////        {
        ////            Revision = skapades.Revision;
        ////            FullkomligtNamn = skapades.FullkomligtNamn;
        ////        }
        ////    }
        ////}
    }
}
