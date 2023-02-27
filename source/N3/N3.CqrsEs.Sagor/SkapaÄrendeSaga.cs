using N3.CqrsEs.SkrivModell.Kommando;
using SmartAnalyzers.CSharpExtensions.Annotations;
using Stateless;
using System.Runtime.Serialization;

namespace N3.CqrsEs.Sagor
{
    public enum SkapaÄrendeSagaTillstånd
    {
        Start = 1,
        T1_Kontrollera_Att_Klient_Finns,
        T2_Skapa_Och_Uppdatera_Gäldenärer,
        Slut,
    }

    public enum SkapaÄrendeSagaAvtryckare
    {
        KontrolleraKlient = 1,
        KontrolleraGäldenärer,
    }

    public enum ExistensKontrollStatus
    {
        EjUtförd = 0,
        OkFinns,
        KontrollMisslyckades,
        FinnsInte,
    }

    public enum SkapaEllerUppdateraStatus
    {
        EjUtförd = 0,
        Skapad,
        Uppdaterad,
    }

    public readonly record struct KontrollDataExisterar(
        ExistensKontrollStatus Status,
        int AntalFörsök = 0
    );

    public readonly record struct KontrollDataSkapaEllerUppdatera(
        ExistensKontrollStatus Status,
        int AntalFörsök = 0
    );

    public enum Exekvering
    {
        Fortsätt,
        Avbryt,
    }

    [InitRequired]
    [DataContract]
    public class SkapaÄrendeSaga
    {
        // för serialisering
        protected SkapaÄrendeSaga() { }

        public SkapaÄrendeSaga(
            UnikIdentifierare identifierare,
            UnikIdentifierare inkassoKlientIdentifierare,
            SkapaEllerUppdateraInkassoGäldenärKommando[] gäldenärer
        )
        {
            TillståndsMaskin = new(() => Tillstånd, (nyttTillstånd) => Tillstånd = nyttTillstånd);
            _ = TillståndsMaskin
                .Configure(SkapaÄrendeSagaTillstånd.Start)
                .Permit(
                    SkapaÄrendeSagaAvtryckare.KontrolleraKlient,
                    SkapaÄrendeSagaTillstånd.T1_Kontrollera_Att_Klient_Finns
                );
            _ = TillståndsMaskin
                .Configure(SkapaÄrendeSagaTillstånd.T1_Kontrollera_Att_Klient_Finns)
                .Permit(
                    SkapaÄrendeSagaAvtryckare.KontrolleraGäldenärer,
                    SkapaÄrendeSagaTillstånd.T2_Skapa_Och_Uppdatera_Gäldenärer
                );

            Identifierare = identifierare;
            InkassoKlientIdentifierare = inkassoKlientIdentifierare;
            Gäldenärer = gäldenärer;
        }

        [DataMember]
        public UnikIdentifierare Identifierare { get; protected set; }

        [DataMember]
        public UnikIdentifierare InkassoKlientIdentifierare { get; protected set; }

        [DataMember]
        public KontrollDataExisterar InkassoKlientKontroll { get; protected set; } =
            new KontrollDataExisterar(ExistensKontrollStatus.EjUtförd);

        [DataMember]
        public SkapaEllerUppdateraInkassoGäldenärKommando[] Gäldenärer { get; protected set; }

        [DataMember]
        public KontrollDataSkapaEllerUppdatera[] GäldenärsKontroller { get; protected set; } =
            Array.Empty<KontrollDataSkapaEllerUppdatera>();

        [DataMember]
        public int GäldenärsKontrollIndex { get; protected set; } = 0;

        [DataMember]
        public SkapaÄrendeSagaTillstånd Tillstånd { get; protected set; } =
            SkapaÄrendeSagaTillstånd.Start;
        private StateMachine<
            SkapaÄrendeSagaTillstånd,
            SkapaÄrendeSagaAvtryckare
        > TillståndsMaskin { get; }

        public async IAsyncEnumerable<Exekvering> Exekvera()
        {
            await ValueTask.CompletedTask;
            if (Tillstånd == SkapaÄrendeSagaTillstånd.Start)
            {
                TillståndsMaskin.Fire(SkapaÄrendeSagaAvtryckare.KontrolleraKlient);
                yield return Exekvering.Fortsätt;
            }
            if (Tillstånd == SkapaÄrendeSagaTillstånd.T1_Kontrollera_Att_Klient_Finns)
            {
                // ingen tillbakarullning behövs
                switch (InkassoKlientKontroll.Status)
                {
                    case ExistensKontrollStatus.FinnsInte:
                        yield return Exekvering.Avbryt;
                        break;
                    case ExistensKontrollStatus.OkFinns:
                        TillståndsMaskin.Fire(SkapaÄrendeSagaAvtryckare.KontrolleraGäldenärer);
                        yield return Exekvering.Fortsätt;
                        break;
                    case ExistensKontrollStatus.EjUtförd:
                        // utför kontroll ...
                        yield return Exekvering.Fortsätt;
                        break;
                    case ExistensKontrollStatus.KontrollMisslyckades:
                        yield return Exekvering.Fortsätt;
                        break;
                    default:
                        throw new ApplicationException("Invalid switch");
                }
            }
            if (Tillstånd == SkapaÄrendeSagaTillstånd.T2_Skapa_Och_Uppdatera_Gäldenärer)
            {
                if (GäldenärsKontrollIndex >= Gäldenärer.Length)
                {
                    yield return Exekvering.Fortsätt;
                }
                var gl = Gäldenärer[GäldenärsKontrollIndex];
                var kontroll = GäldenärsKontroller[GäldenärsKontrollIndex];
                switch (kontroll.Status)
                {
                    case ExistensKontrollStatus.EjUtförd:
                        break;
                    case ExistensKontrollStatus.OkFinns:
                        break;
                    case ExistensKontrollStatus.KontrollMisslyckades:
                        break;
                    case ExistensKontrollStatus.FinnsInte:
                        break;
                    default:
                        throw new ApplicationException("Invalid switch");
                }
            }
        }
    }
}
