using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FeatBit.ClientSdk.Concurrent
{
    public class FeatureFlagsCollection
    {
        private static FeatureFlagsCollection _instance;
        private static readonly object _lock = new object();

        public ConcurrentDictionary<String, FeatureFlag> _dictionary;

        // Private constructor ensures that an object cannot be created outside of the class
        private FeatureFlagsCollection()
        {
            _dictionary = new ConcurrentDictionary<String, FeatureFlag>();
        }

        // Public static method to get the instance of the class
        public static FeatureFlagsCollection Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new FeatureFlagsCollection();
                        }
                    }
                }
                return _instance;
            }
        }

        // Instance methods follow...

        public void InitOrUpdateCollection(List<FeatureFlag> featureFlags)
        {
            foreach(var item in featureFlags)
            {
                _dictionary.AddOrUpdate(item.Id, item, (existingKey, existingValue) => item);
            }
        }
        
        public bool AddOrUpdate(String key, FeatureFlag value)
        {
            return _dictionary.AddOrUpdate(key, value, (existingKey, existingValue) => value) != null;
        }

        public bool TryGetValue(String key, out FeatureFlag value)
        {
            return _dictionary.TryGetValue(key, out value);
        }

        public bool TryRemove(String key)
        {
            return _dictionary.TryRemove(key, out _);
        }

        public List<FeatureFlag> GetAllLatestFeatureFlags()
        {
            return _dictionary.Values.ToList();
        }

        public void Clear()
        {
            _dictionary.Clear();
        }

        public FeatureFlag this[String key]
        {
            get => _dictionary[key];
            set => AddOrUpdate(key, value);
        }

        public int Count => _dictionary.Count;
    }
}
