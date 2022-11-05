using System.Collections.Concurrent;

namespace Nereid.Services
{
    public class DataStore
    {
        public ConcurrentDictionary<string, List<byte[]>> Lists { get; } = new ConcurrentDictionary<string, List<byte[]>>();

        public bool AddList(string name)
        {
            return Lists.TryAdd(name, new List<byte[]>());
        }

        public List<byte[]>? GetList(string name)
        {
            var gotten = Lists.TryGetValue(name, out List<byte[]>? result);
            if (!gotten)
            {
                return null;
            }

            return result;
        }

        public bool AppendToList(string name, byte[] data)
        {
            if (GetList(name) is List<byte[]> actual)
            {
                actual.Add(data);
                return true;
            }

            return false;
        }
    }
}
