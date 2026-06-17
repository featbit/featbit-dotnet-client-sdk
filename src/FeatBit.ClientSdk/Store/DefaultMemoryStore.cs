using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using FeatBit.Sdk.Client.Model;

namespace FeatBit.Sdk.Client.Store
{
    public class DefaultMemoryStore : IMemoryStore
    {
        private const string FlagArchivedReason = "flag archived";

        private readonly ConcurrentDictionary<string, FeatureFlag> _items;

        public DefaultMemoryStore(IEnumerable<FeatureFlag> bootstrap)
        {
            var kvs = bootstrap.Select(flag => new KeyValuePair<string, FeatureFlag>(flag.Id, flag));
            _items = new ConcurrentDictionary<string, FeatureFlag>(kvs);
        }

        public FeatureFlag Get(string key)
        {
            if (_items.TryGetValue(key, out var flag))
            {
                // archived flag is treated as not found
                return flag.MatchReason == FlagArchivedReason ? null : flag;
            }

            return null;
        }

        public ICollection<FeatureFlag> GetAll() => _items.Values
            .Where(flag => flag.MatchReason != FlagArchivedReason)
            .ToArray();

        public void Upsert(FeatureFlag flag)
        {
            FlagValueChangedEvent theEvent = null;

            _items.AddOrUpdate(
                flag.Id,
                addValueFactory: _ =>
                {
                    theEvent = new FlagValueChangedEvent(flag.Id, null, flag.Variation);
                    return flag;
                },
                updateValueFactory: (_, existingFlag) =>
                {
                    if (existingFlag.Variation != flag.Variation)
                    {
                        theEvent = new FlagValueChangedEvent(flag.Id, existingFlag.Variation, flag.Variation);
                    }

                    return flag;
                }
            );

            if (theEvent != null)
            {
                FlagValueChanged?.Invoke(this, theEvent);
            }
        }

        public event EventHandler<FlagValueChangedEvent> FlagValueChanged;
    }
}