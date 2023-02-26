using N3.CqrsEs.Gemensam.Händelser;
using N3.CqrsEs.LäsModell.HändelseHantering;
using N3.CqrsEs.Ramverk;

namespace N3.CqrsEs.Test.TestTjänster
{
    public class MinnesBaseradHändelseKassa : IHändelseKassa
    {
        private readonly Dictionary<UnikIdentifierare, List<IHändelse>> _inMemoryDb = new();

        public MinnesBaseradHändelseKassa(
            IEnumerable<IHändelseMottagare<InkassoÄrendeSkapades>> InkassoÄrendeSkapadesMottagare,
            IEnumerable<
                IHändelseMottagare<InkassoÄrendeBlevTilldelatÄrendeNummer>
            > InkassoÄrendeFickÄrendeNummerMottagare
        )
        {
            this.InkassoÄrendeSkapadesMottagare = InkassoÄrendeSkapadesMottagare;
            this.InkassoÄrendeFickÄrendeNummerMottagare = InkassoÄrendeFickÄrendeNummerMottagare;
        }

        public IEnumerable<
            IHändelseMottagare<InkassoÄrendeSkapades>
        > InkassoÄrendeSkapadesMottagare { get; }
        public IEnumerable<
            IHändelseMottagare<InkassoÄrendeBlevTilldelatÄrendeNummer>
        > InkassoÄrendeFickÄrendeNummerMottagare { get; }

        public async Task<IEnumerable<IHändelse>> Hämta<T>(AggregatStrömIdentifierare<T> ström)
            where T : IAggregatBas
        {
            await ValueTask.CompletedTask;
            var hittad = _inMemoryDb.TryGetValue(ström.Identifierare, out var händelser);
            if (hittad && händelser is not null)
            {
                return händelser;
            }

            return new List<IHändelse>();
        }

        public async Task Registrera<T>(AggregatStrömIdentifierare<T> ström, IHändelse händelse)
            where T : IAggregatBas
        {
            await ValueTask.CompletedTask;
            var hittad = _inMemoryDb.TryGetValue(ström.Identifierare, out var händelser);
            if (!hittad || händelser is null)
            {
                händelser = new List<IHändelse>();
                _inMemoryDb[ström.Identifierare] = händelser;
            }

            händelser.Add(händelse);

            if (händelse is InkassoÄrendeSkapades inkassoÄrendeSkapades)
            {
                foreach (var mottagare in InkassoÄrendeSkapadesMottagare)
                {
                    await mottagare.TaEmot(inkassoÄrendeSkapades);
                }
            }
            if (händelse is InkassoÄrendeBlevTilldelatÄrendeNummer inkassoÄrendeFickÄrendeNummer)
            {
                foreach (var mottagare in InkassoÄrendeFickÄrendeNummerMottagare)
                {
                    await mottagare.TaEmot(inkassoÄrendeFickÄrendeNummer);
                }
            }
        }
    }
}
