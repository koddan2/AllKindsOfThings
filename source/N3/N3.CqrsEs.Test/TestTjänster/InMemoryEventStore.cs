
using N3.CqrsEs.Ramverk;

namespace N3.CqrsEs.Test.TestTjänster
{
    public class InMemoryEventStore : IHändelseKassa
    {
        private readonly Dictionary<UnikIdentifierare, List<IHändelse>> _inMemoryDb = new();

        public InMemoryEventStore()
        {
        }

        public IEnumerable<IHändelse> Hämta(UnikIdentifierare identifierare)
        {
            return _inMemoryDb[identifierare];
        }

        public async Task Registrera<T>(AggregatStrömIdentifierare<T> ström, IHändelse händelse)
        {
            await ValueTask.CompletedTask;
            _inMemoryDb[ström.ByggStrömIdentifierare()].Add(händelse);
        }
    }
}
