using N3.CqrsEs.Gemensam.Händelser;
using N3.CqrsEs.LäsModell.HändelseHantering;
using N3.CqrsEs.Ramverk;

namespace N3.CqrsEs.Test.TestTjänster
{
    public class MinnesBaseradHändelseKassa : IHändelseKassa
    {
        private readonly Dictionary<UnikIdentifierare, List<IHändelse>> _inMemoryDb = new();

        public MinnesBaseradHändelseKassa(
            IEnumerable<IHändelseMottagare<InkassoÄrendeSkapades>> InkassoÄrendeSkapadesMottagare
        )
        {
            this.InkassoÄrendeSkapadesMottagare = InkassoÄrendeSkapadesMottagare;
        }

        public IEnumerable<
            IHändelseMottagare<InkassoÄrendeSkapades>
        > InkassoÄrendeSkapadesMottagare { get; }

        public IEnumerable<IHändelse> Hämta<T>(AggregatStrömIdentifierare<T> ström)
        {
            var hittad = _inMemoryDb.TryGetValue(ström.Identifierare, out var händelser);
            if (hittad && händelser is not null)
            {
                return händelser;
            }

            return new List<IHändelse>();
        }

        public async Task Registrera<T>(AggregatStrömIdentifierare<T> ström, IHändelse händelse)
        {
            await ValueTask.CompletedTask;
            var hittad = _inMemoryDb.TryGetValue(ström.Identifierare, out var händelser);
            if (!hittad || händelser is null)
            {
                händelser = new List<IHändelse>();
            }
            händelser.Add(händelse);
            _inMemoryDb[ström.Identifierare] = händelser;
            if (händelse is InkassoÄrendeSkapades typadHändelse)
            {
                foreach (var mottagare in InkassoÄrendeSkapadesMottagare)
                {
                    await mottagare.TaEmot(typadHändelse);
                }
            }
        }
    }
}
