using System.Collections.Generic;
using System.Linq;
using FeatBit.Sdk.Client.Model;

namespace FeatBit.Sdk.Client.Store
{
    public class DefaultMemoryStore : IMemoryStore
    {
        private readonly object _writeLock = new object();

        private volatile Dictionary<string, FeatureFlag> _items =
            new Dictionary<string, FeatureFlag>();

        public void Populate(IEnumerable<FeatureFlag> flags)
        {
            lock (_writeLock)
            {
                _items = flags.ToDictionary(flag => flag.Id, flag => flag);
            }
        }

        public FeatureFlag Get(string key)
        {
            return _items.TryGetValue(key, out var flag) ? flag : null;
        }

        public ICollection<FeatureFlag> GetAll() => _items.Values;

        public void Upsert(FeatureFlag flag)
        {
            lock (_writeLock)
            {
                _items[flag.Id] = flag;
            }
        }
    }
}